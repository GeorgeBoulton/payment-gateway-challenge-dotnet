using System.Text.Json.Serialization;

namespace PaymentGateway.DAL.DAOs;

public record PaymentResponseDao
(
    bool Authorized,
    [property: JsonPropertyName("authorization_code")]
    string? AuthorizationCode
);