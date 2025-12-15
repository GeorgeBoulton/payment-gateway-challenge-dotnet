using FluentAssertions;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.DAL.UnitTests.Mappers;

[TestFixture]
public class PaymentResponseMapperTests
{
    private readonly PaymentResponseMapper _sut = new();
    
    [Test]
    public void Map_GivenPaymentResponseDao_ReturnsPaymentResponse()
    {
        // Arrange
        var paymentResponse = ModelHelpers.CreatePaymentResponseDao();
        
        // Act
        var result = _sut.Map(paymentResponse);
            
        // Assert
        var expected = new PaymentResponse(
            paymentResponse.Authorized,
            paymentResponse.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
}