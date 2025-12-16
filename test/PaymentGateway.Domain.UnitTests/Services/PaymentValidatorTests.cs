using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using PaymentGateway.Config;
using PaymentGateway.Domain.Services;
using PaymentGateway.Tests.Shared.Extensions;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Domain.UnitTests.Services;

[TestFixture]
    public class PaymentValidatorTests
    {
        private readonly ILogger<PaymentValidator> _logger = Substitute.For<ILogger<PaymentValidator>>();
        private readonly IOptions<PaymentGatewayOptions> _options = Substitute.For<IOptions<PaymentGatewayOptions>>();
        
        private PaymentValidator _sut;
        
        [SetUp]
        public void SetUp()
        {
            _options.Value.Returns(new PaymentGatewayOptions { ApprovedCurrencies = ["GBP", "EUR", "USD"] });
            _sut = new PaymentValidator(_options, _logger);
        }

        [Test]
        public void IsPaymentValid_ApprovedCurrencyAndExpiryInPast_ReturnsFalse()
        {
            // Arrange
            var request = ModelHelpers.CreatePaymentRequest(
                expiryMonth: 1,
                expiryYear: 2020,
                currency: "GBP");

            // Act
            var result = _sut.IsPaymentValid(request);

            // Assert
            result.Should().BeFalse();
            _logger.ReceivedLog(LogLevel.Warning, $"Card has expired. Payment will be rejected for card ending in: {request.CardNumber[^4..]}");
        }

        [Test]
        public void IsPaymentValid_ApprovedCurrencyAndExpiryInFuture_ReturnsTrue()
        {
            var future = DateTime.UtcNow.AddMonths(1);
            var request = ModelHelpers.CreatePaymentRequest(
                expiryMonth: future.Month,
                expiryYear: future.Year,
                currency: "GBP");

            var result = _sut.IsPaymentValid(request);

            result.Should().BeTrue();
        }

        [Test]
        public void IsPaymentValid_ApprovedCurrencyAndExpiryThisMonth_ReturnsTrue()
        {
            var now = DateTime.UtcNow;
            var request = ModelHelpers.CreatePaymentRequest(
                expiryMonth: now.Month,
                expiryYear: now.Year,
                currency: "GBP");

            var result = _sut.IsPaymentValid(request);

            result.Should().BeTrue();
        }

        [TestCase("AUD")]
        [TestCase("CAD")]
        [TestCase("JPY")]
        public void IsPaymentValid_UnapprovedCurrencyAndExpiryInFuture_ReturnsFalse(string currency)
        {
            var future = DateTime.UtcNow.AddMonths(1);
            var request = ModelHelpers.CreatePaymentRequest(
                expiryMonth: future.Month,
                expiryYear: future.Year,
                currency: currency);

            var result = _sut.IsPaymentValid(request);

            result.Should().BeFalse();
            _logger.ReceivedLog(LogLevel.Warning, $"Currency is not approved. Payment will be rejected for card ending in: {request.CardNumber[^4..]}");
        }

        [Test]
        public void IsPaymentValid_ExpiredAndUnapprovedCurrency_ReturnsFalse()
        {
            var request = ModelHelpers.CreatePaymentRequest(
                expiryMonth: 1,
                expiryYear: 2020,
                currency: "AUD");

            var result = _sut.IsPaymentValid(request);

            result.Should().BeFalse();
            _logger.ReceivedLog(LogLevel.Warning, $"Card has expired. Payment will be rejected for card ending in: {request.CardNumber[^4..]}");
            _logger.ReceivedLog(LogLevel.Warning, $"Currency is not approved. Payment will be rejected for card ending in: {request.CardNumber[^4..]}");
        }
    }