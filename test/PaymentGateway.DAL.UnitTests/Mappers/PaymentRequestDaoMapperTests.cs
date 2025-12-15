using FluentAssertions;
using PaymentGateway.DAL.DAOs;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.UnitTests.Helpers;

namespace PaymentGateway.DAL.UnitTests.Mappers;

[TestFixture]
public class PaymentRequestDaoMapperTests
{
    private readonly PaymentRequestDaoMapper _sut = new();
    
    [Test]
    public void Map_GivenPaymentRequest_ReturnsPaymentRequestDao_WithCorrectlyFormattedDate()
    {
        // Arrange
        var paymentRequest1 = ModelHelpers.CreatePaymentRequest(expiryMonth: 9, expiryYear: 2024);
        var paymentRequest2 = ModelHelpers.CreatePaymentRequest(expiryMonth: 12, expiryYear: 2027);
        
        // Act
        var result1 = _sut.Map(paymentRequest1);
        var result2 = _sut.Map(paymentRequest2);
            
        // Assert
        var expected1 = new PaymentRequestDao(
            paymentRequest1.CardNumber,
            "09/2024",
            paymentRequest1.Currency,
            paymentRequest1.Amount, 
            paymentRequest1.Cvv);
        
        var expected2 = new PaymentRequestDao(
            paymentRequest2.CardNumber,
            "12/2027",
            paymentRequest2.Currency,
            paymentRequest2.Amount, 
            paymentRequest2.Cvv);
            
        result1.Should().BeEquivalentTo(expected1);
        result2.Should().BeEquivalentTo(expected2);
    }
}