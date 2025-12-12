using PaymentGateway.DAL.Entities;

namespace PaymentGateway.DAL.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    public List<PaymentEntity> Payments = new();
    
    public void Add(PaymentEntity paymentEntity)
    {
        Payments.Add(paymentEntity);
    }

    public PaymentEntity Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}