using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Factories;

public class PaymentFactory : IPaymentFactory
{
    public Payment CreateRejected(Guid id, PaymentRequest request) =>
        new(
            id,
            request.CardNumber,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.Currency,
            request.Amount,
            PaymentStatus.Rejected);

    public Payment CreateFromResponse(Guid id, PaymentRequest request, PaymentResponse response) =>
        new(
            id,
            request.CardNumber,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.Currency,
            request.Amount,
            response.Authorized
                ? PaymentStatus.Authorized
                : PaymentStatus.Declined,
            response.AuthorizationCode);
}