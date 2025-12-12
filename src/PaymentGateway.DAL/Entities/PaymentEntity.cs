namespace PaymentGateway.DAL.Entities;

public record PaymentEntity(
    Guid Id,
    Status Status,
    int CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string? AuthorizationCode
    );