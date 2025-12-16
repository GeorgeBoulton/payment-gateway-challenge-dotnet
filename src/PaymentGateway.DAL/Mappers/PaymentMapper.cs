using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public class PaymentMapper(IStatusMapper statusMapper) : IPaymentMapper
{
    public PaymentEntity Map(Payment payment) =>
        new(
            payment.Id,
            statusMapper.Map(payment.Status),
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount,
            payment.AuthorizationCode ?? null
        );

    public Payment Map(PaymentEntity payment) =>
        new(
            payment.Id, 
            payment.CardNumber, 
            payment.ExpiryMonth, 
            payment.ExpiryYear, 
            payment.Currency,
            payment.Amount, 
            statusMapper.Map(payment.Status), 
            payment.AuthorizationCode ?? null);
}