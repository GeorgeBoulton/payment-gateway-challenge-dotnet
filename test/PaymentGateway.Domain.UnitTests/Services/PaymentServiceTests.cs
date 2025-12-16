using AutoFixture;

using FluentAssertions;

using NSubstitute;

using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Factories;
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
    private readonly IPaymentValidator _paymentValidator = Substitute.For<IPaymentValidator>();
    private readonly IPaymentFactory _paymentFactory = Substitute.For<IPaymentFactory>();

    private PaymentService _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentService(
            _paymentProcessor,
            _paymentDataProcessor,
            _paymentValidator,
            _paymentFactory);
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
    public async Task ProcessPaymentAsync_GivenInvalidPayment_CallsCreateRejectedAndReturns()
    {
        // Arrange
        var request = ModelHelpers.CreatePaymentRequest();
        var factoryRejectedPayment = ModelHelpers.CreatePayment(cardNumber: request.CardNumber);

        _paymentValidator.IsPaymentValid(request).Returns(false);
        _paymentFactory
            .CreateRejected(Arg.Any<Guid>(), request)
            .Returns(factoryRejectedPayment);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Should().BeEquivalentTo(factoryRejectedPayment, options =>
            options.Excluding(p => p.CardNumber));
        result.CardNumber.Should().Be($"**** **** **** {request.CardNumber[^4..]}");

        // Ensure the payment processor was never called
        await _paymentProcessor
            .DidNotReceive()
            .ProcessPayment(Arg.Any<PaymentRequest>());

        // Ensure no payment was stored
        _paymentDataProcessor
            .DidNotReceive()
            .StorePayment(Arg.Any<Payment>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ProcessPaymentAsync_GivenValidPayment_ProcessesAndReturnsPayment(bool authorized)
    {
        // Arrange
        var request = ModelHelpers.CreatePaymentRequest();
        var response = new PaymentResponse(authorized, authorized ? _fixture.Create<string>() : null);
        var factoryPayment = ModelHelpers.CreatePayment(cardNumber: request.CardNumber);

        _paymentValidator.IsPaymentValid(request).Returns(true);
        _paymentProcessor
            .ProcessPayment(request)
            .Returns(response);
        _paymentFactory
            .CreateFromResponse(Arg.Any<Guid>(), request, response)
            .Returns(factoryPayment);

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Should().BeEquivalentTo(factoryPayment, options =>
            options.Excluding(p => p.CardNumber));
        result.CardNumber.Should().Be($"**** **** **** {request.CardNumber[^4..]}");

        _paymentDataProcessor
            .Received(1)
            .StorePayment(factoryPayment);

        _paymentFactory
            .Received(1)
            .CreateFromResponse(Arg.Any<Guid>(), request, response);
    }
}