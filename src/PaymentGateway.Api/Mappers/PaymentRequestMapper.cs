using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public class PaymentRequestMapper : IPaymentRequestMapper
{
    public PaymentRequest Map(PostPaymentRequest postPaymentRequest) =>
        new(
            postPaymentRequest.CardNumber,
            postPaymentRequest.ExpiryMonth,
            postPaymentRequest.ExpiryYear,
            postPaymentRequest.Currency,
            postPaymentRequest.Amount,
            postPaymentRequest.Cvv);
}