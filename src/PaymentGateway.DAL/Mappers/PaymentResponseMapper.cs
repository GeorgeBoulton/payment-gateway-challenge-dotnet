using PaymentGateway.DAL.DAOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public class PaymentResponseMapper : IPaymentResponseMapper
{
    // Map into Domain model to be handled in domain
    public PaymentResponse Map(PaymentResponseDao paymentResponseDao) =>
        new(paymentResponseDao.Authorized,
            paymentResponseDao.AuthorizationCode);
}