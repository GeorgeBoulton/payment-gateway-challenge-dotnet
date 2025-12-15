using AutoFixture;

using FluentAssertions;

using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Api.UnitTests.Mappers;

[TestFixture]
public class GetPaymentResponseMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly GetPaymentResponseMapper _sut = new();
    
    [TestCase("Authorized", PaymentStatus.Authorized)]
    [TestCase("Declined", PaymentStatus.Declined)]
    [TestCase("Rejected", PaymentStatus.Rejected)]
    public void Map_GivenPayment_ReturnsGetPaymentResponseWithCorrectFields(string statusString, PaymentStatus expectedStatus)
    {
        // Arrange
        var payment = _fixture.Build<Payment>()
            .With(p => p.Status, statusString)
            .Create();

        // Act
        var result = _sut.Map(payment);

        // Assert
        var expected = new GetPaymentResponse(
            payment.Id,
            expectedStatus,
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount
        );

        result.Should().BeEquivalentTo(expected);
    }
}