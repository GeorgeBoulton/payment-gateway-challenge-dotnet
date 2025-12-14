using PaymentGateway.DAL.DAOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public class PaymentResponseMapper : IPaymentResponseMapper
{
    public PaymentResponse Map(PaymentResponseDao paymentResponseDao) =>
        new(paymentResponseDao.Authorized,
            paymentResponseDao.AuthorizationCode);
}