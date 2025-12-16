using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public class PaymentValidator : IPaymentValidator
{
    // Hashset O(1) lookup
    private static readonly HashSet<string> ApprovedCurrencies = ["GBP", "EUR", "USD"];
    
    public bool IsPaymentValid(PaymentRequest paymentRequest)
    {
        return 
            IsExpiryInFuture(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear) &&
            IsCurrencyApproved(paymentRequest.Currency);
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
