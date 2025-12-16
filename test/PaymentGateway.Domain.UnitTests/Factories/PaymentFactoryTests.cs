using FluentAssertions;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Factories;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Domain.UnitTests.Factories;

[TestFixture]
public class PaymentFactoryTests
{
    private readonly PaymentFactory _sut = new();

    [Test]
    public void CreateRejected_GivenIdAndRequest_ReturnsRejectedPayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = ModelHelpers.CreatePaymentRequest(
            cardNumber: "1234567812345678",
            expiryMonth: 12,
            expiryYear: 2030,
            currency: "GBP",
            amount: 100);

        // Act
        var result = _sut.CreateRejected(id, request);

        // Assert
        result.Id.Should().Be(id);
        result.CardNumber.Should().Be(request.CardNumber);
        result.ExpiryMonth.Should().Be(request.ExpiryMonth);
        result.ExpiryYear.Should().Be(request.ExpiryYear);
        result.Currency.Should().Be(request.Currency);
        result.Amount.Should().Be(request.Amount);
        result.Status.Should().Be(PaymentStatus.Rejected);
        result.AuthorizationCode.Should().BeNull();
    }

    [Test]
    public void CreateFromResponse_WhenAuthorized_ReturnsAuthorizedPayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = ModelHelpers.CreatePaymentRequest(
            cardNumber: "1234567812345678",
            expiryMonth: 12,
            expiryYear: 2030,
            currency: "GBP",
            amount: 200);
        var response = new PaymentResponse(true, Guid.NewGuid().ToString());

        // Act
        var result = _sut.CreateFromResponse(id, request, response);

        // Assert
        result.Id.Should().Be(id);
        result.CardNumber.Should().Be(request.CardNumber);
        result.ExpiryMonth.Should().Be(request.ExpiryMonth);
        result.ExpiryYear.Should().Be(request.ExpiryYear);
        result.Currency.Should().Be(request.Currency);
        result.Amount.Should().Be(request.Amount);
        result.Status.Should().Be(PaymentStatus.Authorized);
        result.AuthorizationCode.Should().Be(response.AuthorizationCode);
    }

    [Test]
    public void CreateFromResponse_WhenDeclined_ReturnsDeclinedPayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = ModelHelpers.CreatePaymentRequest(
            cardNumber: "1234567812345678",
            expiryMonth: 12,
            expiryYear: 2030,
            currency: "USD",
            amount: 50);
        var response = new PaymentResponse(false, null);

        // Act
        var result = _sut.CreateFromResponse(id, request, response);

        // Assert
        result.Id.Should().Be(id);
        result.CardNumber.Should().Be(request.CardNumber);
        result.ExpiryMonth.Should().Be(request.ExpiryMonth);
        result.ExpiryYear.Should().Be(request.ExpiryYear);
        result.Currency.Should().Be(request.Currency);
        result.Amount.Should().Be(request.Amount);
        result.Status.Should().Be(PaymentStatus.Declined);
        result.AuthorizationCode.Should().BeNull();
    }
}