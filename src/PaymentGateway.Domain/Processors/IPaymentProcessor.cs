using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Processors;

public interface IPaymentProcessor
{
    Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
}