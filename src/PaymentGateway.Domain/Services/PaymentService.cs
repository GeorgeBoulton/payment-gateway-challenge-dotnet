using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.Domain.Services;

public class PaymentService(
    IPaymentProcessor paymentProcessor,
    IPaymentDataProcessor paymentDataProcessor) : IPaymentService
{
    private static readonly string[] _approvedCurrencies = ["GBP", "EUR", "USD"];
    
    public Payment? GetPayment(Guid id)
    {
        var payment = paymentDataProcessor.RetrievePayment(id);

        return payment is not null ? MaskCardNumber(payment) : null; 
    }
    
    // todo do we want to create some kind of tryRequest that we can wrap return types?
    // the idea is its then you can be like TryResult<T>
    // so its like TryResult.Fail, T (Payment) = null
    // or TryResult.Pass, T (Payment) = result
    // then we dont have to throw exceptions and can return more accurate error codes.
    // things can go wrong but theyre not exceptional.
    public async Task<Payment> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        var paymentId = Guid.NewGuid();
        
        if (!IsExpiryInFuture(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear))
        {
            return new Payment(
                paymentId,
                paymentRequest.CardNumber,
                paymentRequest.ExpiryMonth,
                paymentRequest.ExpiryYear,
                paymentRequest.Currency,
                paymentRequest.Amount,
                "Rejected");
        }

        if (!IsCurrencyApproved(paymentRequest.Currency))
        {
            return new Payment(
                paymentId,
                paymentRequest.CardNumber,
                paymentRequest.ExpiryMonth,
                paymentRequest.ExpiryYear,
                paymentRequest.Currency,
                paymentRequest.Amount,
                "Rejected");
        }
        
        var paymentResponse = await paymentProcessor.ProcessPayment(paymentRequest);
        
        var payment = new Payment(
            paymentId,
            paymentRequest.CardNumber,
            paymentRequest.ExpiryMonth,
            paymentRequest.ExpiryYear,
            paymentRequest.Currency,
            paymentRequest.Amount,
            paymentResponse.Authorized ? "Authorized" : "Declined", // todo bad. should have method for this
            paymentResponse.AuthorizationCode);
        
        paymentDataProcessor.StorePayment(payment);
        
        return MaskCardNumber(payment);
    }

    private static bool IsExpiryInFuture(int expiryMonth, int expiryYear)
    {
        // First day of the month after the expiry month
        var expiryDate = new DateOnly(expiryYear, expiryMonth, 1)
            .AddMonths(1);

        return expiryDate > DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private static bool IsCurrencyApproved(string currencyCode)
    {
        if (_approvedCurrencies.Contains(currencyCode))
        {
            return true;
        }

        return false;
    }
    
    private static Payment MaskCardNumber(Payment payment)
    {
        return payment with { CardNumber = $"**** **** **** {payment.CardNumber[^4..]}" };
    }
}