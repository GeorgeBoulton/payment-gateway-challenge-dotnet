namespace PaymentGateway.Api.Models.Responses;

public record PaymentResponse(
    Guid Id,
    PaymentStatus Status,
    string MaskedCardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount
);