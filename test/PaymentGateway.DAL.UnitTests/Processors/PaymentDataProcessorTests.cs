using AutoFixture;

using FluentAssertions;

using NSubstitute;

using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Processors;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.DAL.UnitTests.Helpers;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.UnitTests.Processors;

[TestFixture]
public class PaymentDataProcessorTests
{
    private readonly Fixture _fixture = new();
    
    private IPaymentsRepository _paymentsRepository;
    private IPaymentMapper _paymentMapper;
    
    private PaymentDataProcessor _sut;

    [SetUp]
    public void SetUp()
    {
        _paymentsRepository = Substitute.For<IPaymentsRepository>();
        _paymentMapper = Substitute.For<IPaymentMapper>();

        _sut = new PaymentDataProcessor(_paymentsRepository, _paymentMapper);
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
    }

    [TestCase("4758 3644 1283 2839", "**** **** **** 2839")]
    [TestCase("1234567898765432"   , "**** **** **** 5432")]
    [TestCase("4000 11112222 3333" , "**** **** **** 3333")]
    [TestCase("457364826738 3324"  , "**** **** **** 3324")]
    [TestCase("4340 111573927773"  , "**** **** **** 7773")]
    public void RetrievePayment_GivenPaymentId_ReturnsMaskedPaymentForAllFormat(string cardNumber, string expectedMasked)
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentEntity = ModelHelpers.CreatePaymentEntity(cardNumber: cardNumber);
        var payment = ModelHelpers.CreatePayment(cardNumber: cardNumber);

        _paymentsRepository.Get(paymentId).Returns(paymentEntity);
        _paymentMapper.Map(paymentEntity).Returns(payment);

        // Act
        var result = _sut.RetrievePayment(paymentId);

        // Assert
        _paymentsRepository.Received(1).Get(paymentId);
        _paymentMapper.Received(1).Map(paymentEntity);
        result.Should().BeEquivalentTo(payment, options => options
            .Excluding(p => p.CardNumber));
        result.CardNumber.Should().BeEquivalentTo(expectedMasked);
    }
}