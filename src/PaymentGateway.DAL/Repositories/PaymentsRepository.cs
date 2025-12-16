using PaymentGateway.DAL.Entities;

namespace PaymentGateway.DAL.Repositories;

// For the sake of this technical test we are imagining this is a properly set up repository that interfaces with a
// database.
public class PaymentsRepository : IPaymentsRepository
{
    public List<PaymentEntity> Payments = new();
    
    public void Add(PaymentEntity paymentEntity)
    {
        Payments.Add(paymentEntity);
    }

    public PaymentEntity? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}