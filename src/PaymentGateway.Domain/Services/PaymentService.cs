using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public class PaymentService : IPaymentService
{
    public async Task<Payment> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        // todo logic definition
        // Ensure expiry month and expiry date combo in future
        
        // Get resulting response from endpoint in a mapped form
        
        // Create a payment
        
        // Add to payment repository
        
        // Return that payment
        
        throw new NotImplementedException();
        
    }
}