using PaymentGateway.DAL.Entities;

namespace PaymentGateway.DAL.Repositories;

public interface IPaymentsRepository
{
    void Add(PaymentEntity paymentEntity);
    PaymentEntity? Get(Guid id);
}