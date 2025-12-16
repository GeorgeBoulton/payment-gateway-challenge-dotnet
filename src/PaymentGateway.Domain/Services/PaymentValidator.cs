using Microsoft.Extensions.Logging;

using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public class PaymentValidator(ILogger<PaymentValidator> logger) : IPaymentValidator
{
    // Hashset O(1) lookup
    private static readonly HashSet<string> ApprovedCurrencies = ["GBP", "EUR", "USD"];
    
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
        // First day of the month after the expiry month
        var expiryDate = new DateOnly(expiryYear, expiryMonth, 1)
            .AddMonths(1);

        // If this is true it is still valid for the remainder of the month
        return expiryDate > DateOnly.FromDateTime(DateTime.UtcNow);
    }
    
    private static bool IsCurrencyApproved(string currencyCode)
    {
        return ApprovedCurrencies.Contains(currencyCode);
    }
}
