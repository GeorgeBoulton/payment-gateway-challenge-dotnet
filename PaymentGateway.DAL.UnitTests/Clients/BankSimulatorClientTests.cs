using AutoFixture;
using FluentAssertions;
using NSubstitute;
using PaymentGateway.DAL.Clients;
using PaymentGateway.DAL.DAOs;

namespace PaymentGateway.DAL.UnitTests.Clients;

[TestFixture]
[TestOf(typeof(BankSimulatorClientTests))]
public class BankSimulatorClientTests
{
    private readonly Fixture _fixture = new();
        
    private readonly IBaseClient _baseClient = Substitute.For<IBaseClient>();

    private BankSimulatorClient _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new BankSimulatorClient(_baseClient);
    }
    
    [Test]
    public async Task ProcessPaymentRequestAsync_GivenPaymentRequest_ReturnsAuthorizedPaymentResponse()
    {
        // Arrange
        var request = _fixture.Create<PaymentRequestDao>();
        var authorizationCode = Guid.NewGuid().ToString();
        var paymentResponse = _fixture.Build<PaymentResponseDao>()
            .With(x => x.Authorized, true)
            .With(x => x.AuthorizationCode, authorizationCode)
            .Create();
        
        _baseClient
            .PostAsync<PaymentRequestDao, PaymentResponseDao>(
                Arg.Is<Uri>(x => x.AbsoluteUri == "http://localhost:8080/payments"),
                request)
            .Returns(Task.FromResult(paymentResponse));
        
        // Act
        var result = await _sut.ProcessPaymentRequestAsync(request);
            
        // Assert
        var expected = new PaymentResponseDao(true, authorizationCode);
            
        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public async Task ProcessPaymentRequestAsync_GivenPaymentRequest_ReturnsUnauthorizedPaymentResponse()
    {
        // Arrange
        var request = _fixture.Create<PaymentRequestDao>();
        var paymentResponse = _fixture.Build<PaymentResponseDao>()
            .With(x => x.Authorized, false)
            .With(x => x.AuthorizationCode, (string?) null)
            .Create();
        
        _baseClient
            .PostAsync<PaymentRequestDao, PaymentResponseDao>(
                Arg.Is<Uri>(x => x.AbsoluteUri == "http://localhost:8080/payments"),
                request)
            .Returns(Task.FromResult(paymentResponse));
        
        // Act
        var result = await _sut.ProcessPaymentRequestAsync(request);
            
        // Assert
        var expected = new PaymentResponseDao(false, null);
            
        result.Should().BeEquivalentTo(expected);
    }
}