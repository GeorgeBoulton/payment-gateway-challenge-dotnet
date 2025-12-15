using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public interface IPaymentService
{
    Payment? GetPayment(Guid id);
    
    Task<Payment> ProcessPaymentAsync(PaymentRequest paymentRequest);
}