namespace PaymentGateway.Application.Exceptions;

public class BankUnavailableException : Exception
{
    public BankUnavailableException()
        : base("The acquiring bank is temporarily unavailable.")
    {
    }

    public BankUnavailableException(string message)
        : base(message)
    {
    }

    public BankUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}