using System.Net;
using AutoFixture;
using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PaymentGateway.DAL.Clients;
using PaymentGateway.DAL.DAOs;
using PaymentGateway.Shared.Exceptions;
using PaymentGateway.Tests.Shared.Extensions;

namespace PaymentGateway.DAL.UnitTests.Clients;

[TestFixture]
public class BankSimulatorClientTests
{
    private readonly Fixture _fixture = new();
        
    private readonly IBaseClient _baseClient = Substitute.For<IBaseClient>();
    private readonly ILogger<BankSimulatorClient> _logger = Substitute.For<ILogger<BankSimulatorClient>>();

    private BankSimulatorClient _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new BankSimulatorClient(_baseClient, _logger);
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
            
        _logger.ReceivedLog(LogLevel.Information, $"Payment processed by bank for card ending in: {request.CardNumber[^4..]}");
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
        
        _logger.ReceivedLog(LogLevel.Information, $"Payment processed by bank for card ending in: {request.CardNumber[^4..]}");
        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public async Task ProcessPaymentRequestAsync_GivenPaymentRequestAndClientThrowsException_ThrowsException()
    {
        // Arrange
        var request = _fixture.Create<PaymentRequestDao>();
        
        _baseClient
            .PostAsync<PaymentRequestDao, PaymentResponseDao>(
                Arg.Is<Uri>(x => x.AbsoluteUri == "http://localhost:8080/payments"),
                request)
            .ThrowsAsync(new HttpRequestException(
                "Service unavailable", null, HttpStatusCode.ServiceUnavailable));
        
        // Act & Assert
        await FluentActions
            .Invoking(() => _sut.ProcessPaymentRequestAsync(request))
            .Should()
            .ThrowAsync<BankUnavailableException>()
            .WithMessage("Bank simulator returned ServiceUnavailable.");

        _logger.ReceivedLog(
            LogLevel.Error,
            $"ServiceUnavailable: Upstream service bank simulator unavailable. No payment processed for card ending in: {request.CardNumber[^4..]}");
    }
}