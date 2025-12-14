using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Entities;

public record PaymentEntity(
    Guid Id,
    Status Status,
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string? AuthorizationCode
    );