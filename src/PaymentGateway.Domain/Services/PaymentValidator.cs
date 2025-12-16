using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGateway.Config;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public class PaymentValidator(
    IOptions<PaymentGatewayOptions> configuration,
    ILogger<PaymentValidator> logger) : IPaymentValidator
{
    public bool IsPaymentValid(PaymentRequest paymentRequest)
    {
        var expiryInFuture = IsExpiryInFuture(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear);
        var currencyApproved = IsCurrencyApproved(paymentRequest.Currency);

        if (!expiryInFuture) logger.LogWarning("Card has expired. Payment will be rejected for card ending in: {Last4}", paymentRequest.CardNumber[^4..]);
        if (!currencyApproved) logger.LogWarning("Currency is not approved. Payment will be rejected for card ending in: {Last4}", paymentRequest.CardNumber[^4..]);
        
        return expiryInFuture && currencyApproved;
    }
    
    private static bool IsExpiryInFuture(int expiryMonth, int expiryYear)
    {
        var expiryDate = new DateOnly(expiryYear, expiryMonth, 1)
            .AddMonths(1);
        
        return expiryDate > DateOnly.FromDateTime(DateTime.UtcNow);
    }
    
    private bool IsCurrencyApproved(string currencyCode)
    {
        return configuration.Value.ApprovedCurrencies.Contains(currencyCode);
    }
}
