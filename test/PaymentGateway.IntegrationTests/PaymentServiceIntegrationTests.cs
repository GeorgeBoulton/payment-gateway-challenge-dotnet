using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PaymentGateway.Config;
using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Processors;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Factories;
using PaymentGateway.Domain.Processors;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.IntegrationTests;

[TestFixture]
public class PaymentServiceIntegrationTests
{
    private IPaymentProcessor _paymentProcessor = null!;
    private IPaymentsRepository _repository = null!;
    private IPaymentMapper _paymentMapper = null!;
    private PaymentDataProcessor _dataProcessor = null!;
    private PaymentValidator _validator = null!;
    private PaymentFactory _factory = null!;
    private ILogger<PaymentService> _logger = null!;
    private PaymentService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        // Mock processor for these tests. It maps and calls upstream. The mappers are unit tested already. The upstream
        // call is the boundary of my system, so it doesn't need to be involved here.
        _paymentProcessor = Substitute.For<IPaymentProcessor>();

        // DAL
        _repository = new PaymentsRepository();
        _paymentMapper = new PaymentMapper(new StatusMapper());

        _dataProcessor = new PaymentDataProcessor(
            _repository,
            _paymentMapper,
            Substitute.For<ILogger<PaymentDataProcessor>>());

        // PaymentService and domain services
        var options = Options.Create(new PaymentGatewayOptions
        {
            ApprovedCurrencies = ["GBP", "EUR", "USD"]
        });
        _validator = new PaymentValidator(
            options,
            Substitute.For<ILogger<PaymentValidator>>());
        _factory = new PaymentFactory();
        _logger = Substitute.For<ILogger<PaymentService>>();
        
        _sut = new PaymentService(
            _paymentProcessor,
            _dataProcessor,
            _validator,
            _factory,
            _logger);
    }

    [Test]
    public async Task ProcessPaymentAsync_ValidPayment_Authorized_StoresAndMasksCard()
    {
        // Arrange
        var request = new PaymentRequest(
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 2026,
            Currency: "GBP",
            Amount: 100,
            Cvv: "123");

        _paymentProcessor
            .ProcessPayment(request)
            .Returns(new PaymentResponse(true, "AUTH123"));

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert (returned result)
        result.Status.Should().Be(PaymentStatus.Authorized);
        result.AuthorizationCode.Should().Be("AUTH123");
        result.CardNumber.Should().Be("**** **** **** 1111");

        // Assert (persisted payment)
        var stored = _repository.Get(result.Id);
        stored.Should().NotBeNull();
        stored.Status.Should().Be(Status.Authorized);
        stored.CardNumber.Should().Be("4111111111111111");
    }

    [Test]
    public async Task ProcessPaymentAsync_ValidPayment_Declined_StoresDeclinedPayment()
    {
        // Arrange
        var request = new PaymentRequest(
            CardNumber: "4000000000000002",
            ExpiryMonth: 12,
            ExpiryYear: 2026,
            Currency: "USD",
            Amount: 50,
            Cvv: "123");

        _paymentProcessor
            .ProcessPayment(request)
            .Returns(new PaymentResponse(false, null));

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be(PaymentStatus.Declined);
        result.CardNumber.Should().Be("**** **** **** 0002");

        var stored = _repository.Get(result.Id);
        stored.Should().NotBeNull();
        stored.Status.Should().Be(Status.Declined);
    }

    [Test]
    public async Task ProcessPaymentAsync_InvalidPayment_DoesNotCallBank_DoesNotStore()
    {
        // Arrange (unsupported currency)
        var request = new PaymentRequest(
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 2026,
            Currency: "AUD",
            Amount: 100,
            Cvv: "123");

        // Act
        var result = await _sut.ProcessPaymentAsync(request);

        // Assert
        result.Status.Should().Be(PaymentStatus.Rejected);
        result.CardNumber.Should().Be("**** **** **** 1111");

        await _paymentProcessor
            .DidNotReceive()
            .ProcessPayment(Arg.Any<PaymentRequest>());

        _repository.Get(result.Id).Should().BeNull();
    }

    [Test]
    public void GetPayment_WhenPaymentExists_ReturnsMaskedPayment()
    {
        // Arrange
        var payment = new Payment(
            Id: Guid.NewGuid(),
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 2026,
            Currency: "GBP",
            Amount: 100,
            Status: PaymentStatus.Authorized,
            AuthorizationCode: "AUTH123");

        _repository.Add(_paymentMapper.Map(payment));

        // Act
        var result = _sut.GetPayment(payment.Id);

        // Assert
        result.Should().NotBeNull();
        result.CardNumber.Should().Be("**** **** **** 1111");
        result.Status.Should().Be(PaymentStatus.Authorized);
    }

    [Test]
    public void GetPayment_WhenPaymentDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _sut.GetPayment(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
