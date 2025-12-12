using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public record PostPaymentRequest(
    [property: Required(ErrorMessage = "Card number is required.")]
    [property: RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be 14-19 digits and numeric.")]
    string CardNumber,

    [property: Required(ErrorMessage = "Expiry date is required.")]
    [property: RegularExpression(@"^(0[1-9]|1[0-2])\/\d{4}$", ErrorMessage = "Expiry date must be in MM/YYYY format.")]
    int ExpiryMonth,
    
    int ExpiryYear,

    [property: Required(ErrorMessage = "Currency is required.")]
    [property: StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters.")]
    string Currency,

    [property: Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer.")]
    int Amount,

    [property: Required(ErrorMessage = "CVV is required.")]
    [property: RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    string Cvv
);
