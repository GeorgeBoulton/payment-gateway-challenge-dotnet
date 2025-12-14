using PaymentGateway.DAL.DAOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public interface IPaymentRequestDaoMapper
{
    PaymentRequestDao Map(PaymentRequest paymentRequest);
}