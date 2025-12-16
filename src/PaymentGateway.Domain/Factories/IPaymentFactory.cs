using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Factories;

public interface IPaymentFactory
{
    Payment CreateRejected(Guid id, PaymentRequest request);
    Payment CreateFromResponse(Guid id, PaymentRequest request, PaymentResponse response);
}