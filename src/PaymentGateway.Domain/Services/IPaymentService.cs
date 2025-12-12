using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(Payment payment);
}