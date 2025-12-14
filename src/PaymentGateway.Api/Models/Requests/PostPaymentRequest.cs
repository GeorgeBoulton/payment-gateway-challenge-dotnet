using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public record PostPaymentRequest(
    [Required(ErrorMessage = "Card number is required.")]
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be 14-19 digits and numeric.")]
    string CardNumber,

    [Required(ErrorMessage = "Expiry month is required.")]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    int ExpiryMonth,

    [Required(ErrorMessage = "Expiry year is required.")]
    [Range(2000, 2100, ErrorMessage = "Expiry year must be a 4-digit number between 2000 and 2100.")]
    int ExpiryYear,

    [Required(ErrorMessage = "Currency is required.")]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be exactly 3 uppercase characters.")]
    string Currency,

    [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer.")]
    int Amount,

    [Required(ErrorMessage = "CVV is required.")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    string Cvv
);
