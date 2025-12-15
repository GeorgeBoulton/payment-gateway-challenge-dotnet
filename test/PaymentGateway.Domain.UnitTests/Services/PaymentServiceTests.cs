using AutoFixture;

using FluentAssertions;

using NSubstitute;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Processors;
using PaymentGateway.Domain.Services;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Domain.UnitTests.Services;

[TestFixture]
public class PaymentServiceTests
{
    private readonly Fixture _fixture = new();
    
    private readonly IPaymentProcessor _paymentProcessor = Substitute.For<IPaymentProcessor>();
    private readonly IPaymentDataProcessor _paymentDataProcessor = Substitute.For<IPaymentDataProcessor>();

    private PaymentService _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentService(
            _paymentProcessor,
            _paymentDataProcessor);
    }

    [Test]
    public void GetPayment_GivenIdAndPaymentExists_ReturnsPaymentWithMaskedCardNumber()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cardNumber = "4758 3644 1283 2839";
        var payment = ModelHelpers.CreatePayment(cardNumber: cardNumber);

        _paymentDataProcessor
            .RetrievePayment(id)
            .Returns(payment);

        // Act
        var result = _sut.GetPayment(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(payment, options =>
            options.Excluding(p => p.CardNumber));
        result.CardNumber.Should().Be("**** **** **** 2839");
    }

    
    [Test]
    public void GetPayment_GivenIdAndPaymentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _paymentDataProcessor
            .RetrievePayment(id)
            .Returns((Payment?)null);

        // Act
        var result = _sut.GetPayment(id);

        // Assert
        result.Should().BeNull();
    }
    
    [Test]
    public async Task ProcessPaymentAsync_GivenExpiredCard_ReturnsRejectedPayment()
    {
        // Arrange
        var request = ModelHelpers.CreatePaymentRequest(
            expiryMonth: 11,
            expiryYear: 2025);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be("Rejected");
        result.CardNumber.Should().EndWith(request.CardNumber[^4..]);

        await _paymentProcessor
            .DidNotReceive()
            .ProcessPayment(Arg.Any<PaymentRequest>());

        _paymentDataProcessor
            .DidNotReceive()
            .StorePayment(Arg.Any<Payment>());
    }
    
    [Test]
    public async Task ProcessPaymentAsync_GivenUnsupportedCurrency_ReturnsRejectedPayment()
    {
        // Arrange
        var request = ModelHelpers.CreatePaymentRequest(currency: "AUD");

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be("Rejected");

        await _paymentProcessor
            .DidNotReceive()
            .ProcessPayment(Arg.Any<PaymentRequest>());

        _paymentDataProcessor
            .DidNotReceive()
            .StorePayment(Arg.Any<Payment>());
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenBankAuthorizes_ReturnsAuthorizedPayment()
    {
        // Arrange
        var authCode = _fixture.Create<string>();
        var request = ModelHelpers.CreatePaymentRequest(currency: "GBP");
        var response = new PaymentResponse(true, authCode);

        _paymentProcessor
            .ProcessPayment(request)
            .Returns(response);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be("Authorized");
        result.AuthorizationCode.Should().Be(authCode);
        result.CardNumber.Should().Be($"**** **** **** {request.CardNumber[^4..]}");

        _paymentDataProcessor
            .Received(1)
            .StorePayment(Arg.Is<Payment>(p =>
                p.Status == "Authorized" &&
                p.AuthorizationCode == authCode));
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenBankDeclines_ReturnsDeclinedPayment()
    {
        // Arrange
        var request = ModelHelpers.CreatePaymentRequest(currency: "USD");
        var response = new PaymentResponse(false, null);

        _paymentProcessor
            .ProcessPayment(request)
            .Returns(response);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be("Declined");

        _paymentDataProcessor
            .Received(1)
            .StorePayment(Arg.Is<Payment>(p =>
                p.Status == "Declined"));
    }


}