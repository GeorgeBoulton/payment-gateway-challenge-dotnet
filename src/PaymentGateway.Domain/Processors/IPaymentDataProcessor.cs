using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Processors;

public interface IPaymentDataProcessor
{
    void StorePayment(Payment payment);

    Payment? RetrievePayment(Guid id);
}