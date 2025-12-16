using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Services;

public interface IPaymentValidator
{
    bool IsPaymentValid(PaymentRequest paymentRequest);
}