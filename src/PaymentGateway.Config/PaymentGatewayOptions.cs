namespace PaymentGateway.Config;

public class PaymentGatewayOptions
{
    public const string SectionName = "PaymentGateway";

    public string BankBaseUrl { get; init; } = string.Empty;
    public HashSet<string> ApprovedCurrencies { get; init; } = [];
}