namespace PaymentGateway.Api.Models.Responses;

// Inherited in case Get and Post diverge from each other. For example, Post could start returning a "Pending" field on
// an approved transaction, much like you'd see on a credit card.
public record PaymentResponse(
    Guid Id,
    PaymentStatus Status,
    string MaskedCardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount
);