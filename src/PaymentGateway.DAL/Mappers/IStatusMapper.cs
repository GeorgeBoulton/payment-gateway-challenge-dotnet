using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public interface IStatusMapper
{
    PaymentStatus Map(Status status);
    Status Map(PaymentStatus paymentStatus);
}