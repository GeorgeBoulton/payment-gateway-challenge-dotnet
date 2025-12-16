using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Processors;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Shared.Extensions;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.DAL.UnitTests.Processors;

[TestFixture]
public class PaymentDataProcessorTests
{
    private readonly Fixture _fixture = new();
    
    private readonly IPaymentsRepository _paymentsRepository = Substitute.For<IPaymentsRepository>();
    private readonly IPaymentMapper _paymentMapper  = Substitute.For<IPaymentMapper>();
    private readonly ILogger<PaymentDataProcessor> _logger = Substitute.For<ILogger<PaymentDataProcessor>>();
    
    private PaymentDataProcessor _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentDataProcessor(_paymentsRepository, _paymentMapper, _logger);
    }

    [Test]
    public void StorePayment_GivenPayment_CallsMapperAndRepository()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        var paymentEntity = _fixture.Create<PaymentEntity>();
        
        _paymentMapper.Map(payment).Returns(paymentEntity);
        
        // Act
        _sut.StorePayment(payment);
        
        // Assert
        _paymentMapper.Received(1).Map(payment);
        _paymentsRepository.Received(1).Add(paymentEntity);
        _logger.ReceivedLog(LogLevel.Information, $"Payment stored with Id: {paymentEntity.Id}");
    }

    [Test]
    public void RetrievePayment_GivenPaymentId_ReturnsPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentEntity = ModelHelpers.CreatePaymentEntity();
        var payment = ModelHelpers.CreatePayment();

        _paymentsRepository.Get(paymentId).Returns(paymentEntity);
        _paymentMapper.Map(paymentEntity).Returns(payment);

        // Act
        var result = _sut.RetrievePayment(paymentId);

        // Assert
        _paymentsRepository.Received(1).Get(paymentId);
        _paymentMapper.Received(1).Map(paymentEntity);
        result.Should().BeEquivalentTo(payment);
    }

    [Test] 
    public void RetrievePayment_GivenPaymentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var paymentGuid = Guid.NewGuid();
        var payment = _fixture.Create<Payment>();
        
        // Act
        var result = _sut.RetrievePayment(paymentGuid);
        
        // Assert
        _paymentMapper.Received(0).Map(payment);
        _logger.ReceivedLog(LogLevel.Information,$"No payment found matching Id: {paymentGuid}");
        result.Should().BeNull();
    }
}