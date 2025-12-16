using AutoFixture;
using FluentAssertions;
using NSubstitute;
using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.DAL.UnitTests.Mappers;

[TestFixture]
public class PaymentMapperTests
{
    private readonly Fixture _fixture = new();
    
    private readonly IStatusMapper _statusMapper = Substitute.For<IStatusMapper>();
    
    private PaymentMapper _sut;
    
    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentMapper(_statusMapper);
    }
    
    [Test]
    public void Map_GivenPayment_ReturnsPaymentEntity()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedStatus = Status.Authorized;
        var payment = ModelHelpers.CreatePayment(
            id: paymentId);
        
        _statusMapper
            .Map(payment.Status)
            .Returns(expectedStatus);
        
        // Act
        var result = _sut.Map(payment);
            
        // Assert
        var expected = new PaymentEntity(
            paymentId,
            expectedStatus,
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
        var expectedStatus = Status.Declined;
        var payment = ModelHelpers.CreatePayment(
            id: paymentId);
        
        // Act
        _statusMapper
            .Map(payment.Status)
            .Returns(expectedStatus);
            
        // Assert
        var expected = new PaymentEntity(
            paymentId,
            expectedStatus,
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount,
            null);

        var result = _sut.Map(payment);
            
        result.Should().BeEquivalentTo(expected);
        result.AuthorizationCode.Should().BeNull();
        result.Status.Should().Be(Status.Declined);
    }
    
    [Test]
    public void Map_GivenPaymentEntity_ReturnsPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedPaymentStatus = PaymentStatus.Authorized;
        var paymentEntity = ModelHelpers.CreatePaymentEntity(
            id: paymentId);
        
        _statusMapper
            .Map(paymentEntity.Status)
            .Returns(expectedPaymentStatus);
        
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
            expectedPaymentStatus,
            paymentEntity.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void Map_GivenPaymentEntityWithNullAuthorization_ReturnsPaymentEntityWithNullAuthorizationCode()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedPaymentStatus = PaymentStatus.Declined;
        var paymentEntity = ModelHelpers.CreatePaymentEntity(
            id: paymentId);
        
        _statusMapper
            .Map(paymentEntity.Status)
            .Returns(expectedPaymentStatus);
        
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
            expectedPaymentStatus,
            paymentEntity.AuthorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
}