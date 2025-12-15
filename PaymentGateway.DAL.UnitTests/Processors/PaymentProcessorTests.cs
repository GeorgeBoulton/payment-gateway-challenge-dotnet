using AutoFixture;

using FluentAssertions;

using NSubstitute;
using PaymentGateway.DAL.Clients;
using PaymentGateway.DAL.DAOs;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Processors;
using PaymentGateway.DAL.UnitTests.Helpers;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.UnitTests.Processors;

[TestFixture]
public class PaymentProcessorTests
{
    private readonly Fixture _fixture = new();
        
    private readonly IBankSimulatorClient _bankSimulatorClient = Substitute.For<IBankSimulatorClient>();
    private readonly IPaymentRequestDaoMapper _paymentRequestDaoMapper = Substitute.For<IPaymentRequestDaoMapper>();
    private readonly IPaymentResponseMapper _paymentResponseMapper = Substitute.For<IPaymentResponseMapper>();

    private PaymentProcessor _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentProcessor(_bankSimulatorClient, _paymentRequestDaoMapper, _paymentResponseMapper);
    }

    [Test]
    public async Task ProcessPayment_GivenPaymentRequest_ReturnsPaymentResponse()
    {
        // Arrange
        var paymentRequest = ModelHelpers.CreatePaymentRequest();
        var paymentRequestDao = _fixture.Create<PaymentRequestDao>();
        var paymentResponseDao = _fixture.Create<PaymentResponseDao>();
        var paymentResponse = _fixture.Create<PaymentResponse>();

        _paymentRequestDaoMapper.Map(paymentRequest).Returns(paymentRequestDao);
        _bankSimulatorClient.ProcessPaymentRequestAsync(paymentRequestDao).Returns(Task.FromResult(paymentResponseDao));
        _paymentResponseMapper.Map(paymentResponseDao).Returns(paymentResponse);

        // Act
        var result = await _sut.ProcessPayment(paymentRequest);
        
        // Assert
        var expected = new PaymentResponse(paymentResponse.Authorized, paymentResponse.AuthorizationCode);
        
        result.Should().BeEquivalentTo(expected);
        
        _paymentRequestDaoMapper.Received(1).Map(paymentRequest);
        await _bankSimulatorClient.Received(1).ProcessPaymentRequestAsync(paymentRequestDao);
        _paymentResponseMapper.Received(1).Map(paymentResponseDao);
    }

    // todo handle exception? or just at top layer?
}