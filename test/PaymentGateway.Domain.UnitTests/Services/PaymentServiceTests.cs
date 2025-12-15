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

}