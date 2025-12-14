using System.Text.Json.Serialization;

namespace PaymentGateway.DAL.DAOs;

public record PaymentResponseDao
(
    [property: JsonPropertyName("authorized")]
    bool Authorized,
    [property: JsonPropertyName("authorization_code")]
    string? AuthorizationCode
);