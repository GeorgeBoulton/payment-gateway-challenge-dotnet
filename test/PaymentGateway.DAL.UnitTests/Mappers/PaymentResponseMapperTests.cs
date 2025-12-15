using FluentAssertions;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.UnitTests.Helpers;
using PaymentGateway.Domain.Entities;

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