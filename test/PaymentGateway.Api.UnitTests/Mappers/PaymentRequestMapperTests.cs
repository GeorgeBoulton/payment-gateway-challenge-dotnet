using AutoFixture;

using FluentAssertions;

using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Api.UnitTests.Mappers;

[TestFixture]
public class PaymentRequestMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly PaymentRequestMapper _sut = new();

    [Test]
    public void Map_GivenPostPaymentRequest_ReturnsPaymentRequestWithCorrectFields()
    {
        // Arrange
        var postPaymentRequest = _fixture.Create<PostPaymentRequest>();
        
        // Act
        var result = _sut.Map(postPaymentRequest);

        // Assert
        var expected = new PaymentRequest(
            postPaymentRequest.CardNumber,
            postPaymentRequest.ExpiryMonth,
            postPaymentRequest.ExpiryYear,
            postPaymentRequest.Currency,
            postPaymentRequest.Amount,
            postPaymentRequest.Cvv
        );

        result.Should().BeEquivalentTo(expected);
    }
}