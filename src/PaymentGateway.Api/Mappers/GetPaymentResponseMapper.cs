using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public class GetPaymentResponseMapper : IGetPaymentResponseMapper
{
    public GetPaymentResponse Map(Payment payment) =>
        new(
            payment.Id,
            Enum.Parse<PaymentStatus>(payment.Status),
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount
        );
}