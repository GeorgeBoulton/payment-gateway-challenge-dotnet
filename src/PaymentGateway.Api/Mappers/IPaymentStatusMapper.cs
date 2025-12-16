using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Mappers;

public interface IPaymentStatusMapper
{
    PaymentStatus Map(Domain.Entities.PaymentStatus paymentStatus);
}