using PaymentGateway.DAL.DAOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public interface IPaymentResponseMapper
{
    PaymentResponse Map(PaymentResponseDao paymentResponseDao);
}