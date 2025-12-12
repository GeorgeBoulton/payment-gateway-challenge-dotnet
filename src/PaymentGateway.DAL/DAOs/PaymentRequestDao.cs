using System.Text.Json.Serialization;

namespace PaymentGateway.DAL.DAOs;

public record PaymentRequestDao(
    [property: JsonPropertyName("card_number")]
    string CardNumber,
    [property: JsonPropertyName("expiry_date")]
    string ExpiryDate,
    string Currency,
    int Amount,
    string Cvv
);