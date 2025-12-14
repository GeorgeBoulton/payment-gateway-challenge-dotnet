namespace PaymentGateway.Api.Models.Responses;

public record PostPaymentResponse(
    Guid Id,
    PaymentStatus Status,
    string MaskedCardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount) : PaymentResponse(
    Id,
    Status,
    MaskedCardNumber,
    ExpiryMonth,
    ExpiryYear,
    Currency,
    Amount);
