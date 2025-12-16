namespace PaymentGateway.Domain.Entities;

public record Payment(
    Guid Id,
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    PaymentStatus Status,
    string? AuthorizationCode = null);