using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public class PostPaymentResponseMapper(IPaymentStatusMapper paymentStatusMapper) : IPostPaymentResponseMapper
{
    public PostPaymentResponse Map(Payment payment) =>
        new(
            payment.Id,
            paymentStatusMapper.Map(payment.Status),
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount
        );
}