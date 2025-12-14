namespace PaymentGateway.Domain.Entities;

public record PaymentResponse(bool Authorized, string? AuthorizationCode);