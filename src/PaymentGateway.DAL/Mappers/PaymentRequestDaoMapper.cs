using PaymentGateway.DAL.DAOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public class PaymentRequestDaoMapper : IPaymentRequestDaoMapper
{
    public PaymentRequestDao Map(PaymentRequest paymentRequest) =>
        new(
            paymentRequest.CardNumber,
            $"{paymentRequest.ExpiryMonth:D2}/{paymentRequest.ExpiryYear}",
            paymentRequest.Currency,
            paymentRequest.Amount,
            paymentRequest.Cvv);
}