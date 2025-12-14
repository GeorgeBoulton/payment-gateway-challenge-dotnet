using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public interface IPaymentMapper
{
    PaymentEntity Map(Payment payment);
    Payment Map(PaymentEntity payment);
}