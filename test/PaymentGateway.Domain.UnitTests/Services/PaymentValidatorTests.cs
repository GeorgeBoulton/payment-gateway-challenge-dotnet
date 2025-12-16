using FluentAssertions;
using PaymentGateway.Domain.Services;
using PaymentGateway.Tests.Shared.Helpers;

namespace PaymentGateway.Domain.UnitTests.Services;

[TestFixture]
    public class PaymentValidatorTests
    {
        private readonly PaymentValidator _sut = new();

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
        }
    }