using FluentAssertions;
using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.DAL.UnitTests.Mappers;

[TestFixture]
public class PaymentMapperTests
{
    private readonly PaymentMapper _sut = new();
    
    [Test]
    public void Map_GivenPayment_ReturnsPaymentEntity()
    {
        var paymentId = Guid.NewGuid();
        var status = Status.Authorized;
        
        // Arrange
        var payment = ModelHelpers.CreatePayment(
            id: paymentId,
            status: status);
        
        // Act
        var result = _sut.Map(payment);
            
        // Assert
        var expected = new PaymentEntity(
            paymentId,
            status,
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount,
            payment.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void Map_GivenPaymentWithNullAuthorization_ReturnsPaymentEntityWithNullAuthorizationCode()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var status = Status.Declined;
        var payment = ModelHelpers.CreatePayment(
            id: paymentId,
            status: status);
        
        // Act
        var result = _sut.Map(payment);
            
        // Assert
        var expected = new PaymentEntity(
            paymentId,
            status,
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount,
            null);
            
        result.Should().BeEquivalentTo(expected);
        result.AuthorizationCode.Should().BeNull();
        result.Status.Should().Be(Status.Declined);
    }
    
    [Test]
    public void Map_GivenPaymentEntity_ReturnsPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var status = Status.Authorized;
        var paymentEntity = ModelHelpers.CreatePaymentEntity(
            id: paymentId,
            status: status);
        
        // Act
        var result = _sut.Map(paymentEntity);
            
        // Assert
        var expected = new Payment(
            paymentId,
            paymentEntity.CardNumber,
            paymentEntity.ExpiryMonth,
            paymentEntity.ExpiryYear,
            paymentEntity.Currency,
            paymentEntity.Amount,
            status.ToString(),
            paymentEntity.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void Map_GivenPaymentEntityWithNullAuthorization_ReturnsPaymentEntityWithNullAuthorizationCode()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var status = Status.Declined;
        var paymentEntity = ModelHelpers.CreatePaymentEntity(
            id: paymentId,
            status: status);
        
        // Act
        var result = _sut.Map(paymentEntity);
            
        // Assert
        var expected = new Payment(
            paymentId,
            paymentEntity.CardNumber,
            paymentEntity.ExpiryMonth,
            paymentEntity.ExpiryYear,
            paymentEntity.Currency,
            paymentEntity.Amount,
            status.ToString(),
            paymentEntity.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
}