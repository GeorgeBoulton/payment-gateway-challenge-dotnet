using AutoFixture;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Services;
using PaymentGateway.Shared.Exceptions;
using PaymentGateway.Tests.Shared.Extensions;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Api.UnitTests.Controllers;

[TestFixture]
public class PaymentsControllerTests
{
    private readonly Fixture _fixture = new();
    
    private readonly IPaymentService _paymentService = Substitute.For<IPaymentService>();
    private readonly IGetPaymentResponseMapper _getPaymentResponseMapper = Substitute.For<IGetPaymentResponseMapper>();
    private readonly IPostPaymentResponseMapper _postPaymentResponseMapper = Substitute.For<IPostPaymentResponseMapper>();
    private readonly IPaymentRequestMapper _paymentRequestMapper = Substitute.For<IPaymentRequestMapper>();
    private readonly ILogger<PaymentsController> _logger = Substitute.For<ILogger<PaymentsController>>();
    
    
    private PaymentsController _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentsController(
            _paymentService,
            _getPaymentResponseMapper,
            _postPaymentResponseMapper,
            _paymentRequestMapper,
            _logger);
    }
    
    [TearDown]
    public void TearDown()
    {
        (_sut as IDisposable).Dispose();
    }
    
    [Test]
    public async Task GetPayment_GivenPaymentId_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payment = ModelHelpers.CreatePayment();
        var response = _fixture.Create<GetPaymentResponse>();

        _paymentService.GetPayment(id).Returns(payment);
        _getPaymentResponseMapper.Map(payment).Returns(response);

        // Act
        var result = await _sut.GetPayment(id);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().BeEquivalentTo(response);
    }
    
    [Test]
    public async Task GetPayment_WhenPaymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _paymentService.GetPayment(id).Returns((Payment?)null);

        // Act
        var result = await _sut.GetPayment(id);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        // todo check message
    }
    
    [Test]
    public async Task PostProcessPayment_GivenValidRequest_ReturnsOk()
    {
        // Arrange
        var postRequest = _fixture.Create<PostPaymentRequest>();
        var paymentRequest = ModelHelpers.CreatePaymentRequest();
        var paymentResponse = _fixture.Create<Payment>();
        var postResponse = _fixture.Create<PostPaymentResponse>();

        _paymentRequestMapper.Map(postRequest).Returns(paymentRequest);
        _paymentService.ProcessPaymentAsync(paymentRequest).Returns(paymentResponse);
        _postPaymentResponseMapper.Map(paymentResponse).Returns(postResponse);

        // Act
        var result = await _sut.PostProcessPayment(postRequest);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().BeEquivalentTo(postResponse);
        // todo check body
    }
    
    [Test]
    public async Task PostProcessPayment_WhenBankUnavailableExceptionThrown_Returns502()
    {
        // Arrange
        var request = _fixture.Create<PostPaymentRequest>();
        var paymentRequest = ModelHelpers.CreatePaymentRequest();

        _paymentRequestMapper.Map(request).Returns(paymentRequest);
        _paymentService
            .ProcessPaymentAsync(paymentRequest)
            .ThrowsAsync(new BankUnavailableException("Down")); // in the case of 502

        // Act
        var result = await _sut.PostProcessPayment(request);

        // Assert
        var status = result.Should().BeOfType<ObjectResult>().Subject;
        status.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        _logger.ReceivedLog(LogLevel.Error, $"Bank simulator is unavailable. Returning {StatusCodes.Status502BadGateway}");
    }
    
    // This should never ever happen, but we'll test for it anyway
    [Test]
    public async Task PostProcessPayment_WhenUnexpectedExceptionThrown_Returns500()
    {
        // Arrange
        var request = _fixture.Create<PostPaymentRequest>();
        var paymentRequest = ModelHelpers.CreatePaymentRequest();
        var exceptionMessage = "Disaster";

        _paymentRequestMapper.Map(request).Returns(paymentRequest);
        _paymentService
            .ProcessPaymentAsync(paymentRequest)
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _sut.PostProcessPayment(request);

        // Assert
        var status = result.Should().BeOfType<ObjectResult>().Subject;
        status.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _logger.ReceivedLog(LogLevel.Critical, $"Unexpected exception occurred: {exceptionMessage}");
    }
    
}