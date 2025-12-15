using AutoFixture;

using PaymentGateway.DAL.DAOs;
using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Tests.Shared.Helpers;

public static class ModelHelpers
{
    private static readonly int _cardLength = 16;
    private static readonly int _cvvLength = 3;
    private static readonly string[] _approvedCurrencies = ["GBP", "EUR", "USD"];
    private static readonly IFixture fixture = new Fixture();

    public static Payment CreatePayment(
        Guid? id = null,
        string? cardNumber = null,
        int? expiryMonth = null,
        int? expiryYear = null,
        string? currency = null,
        int? amount = null,
        Status? status = null,
        string? authorizationCode = null)
    {
        var localStatus = status ?? fixture.Create<Status>();

        return new Payment(
            id ?? fixture.Create<Guid>(),
            cardNumber ?? GenerateNumberSequence(_cardLength),
            expiryMonth ?? GenerateExpiryMonth(),
            expiryYear ?? GenerateExpiryYear(),
            currency ?? GetCurrency(),
            amount ?? fixture.Create<int>(),
            (status ?? localStatus).ToString(),
            // Setup authorization code to setup correctly corresponding to status. Can provide if you wish.
            authorizationCode ?? (localStatus == Status.Authorized ? fixture.Create<Guid>().ToString() : null)
        );
    }
    
    public static PaymentEntity CreatePaymentEntity(
        Guid? id = null,
        string? cardNumber = null,
        int? expiryMonth = null,
        int? expiryYear = null,
        string? currency = null,
        int? amount = null,
        Status? status = null,
        string? authorizationCode = null)
    {
        var localStatus = status ?? fixture.Create<Status>();

        return new PaymentEntity(
            id ?? fixture.Create<Guid>(),
            status ?? localStatus,
            cardNumber ?? GenerateNumberSequence(_cardLength),
            expiryMonth ?? GenerateExpiryMonth(),
            expiryYear ?? GenerateExpiryYear(),
            currency ?? GetCurrency(),
            amount ?? fixture.Create<int>(),
            authorizationCode ?? (localStatus == Status.Authorized ? fixture.Create<Guid>().ToString() : null)
        );
    }
    
    public static PaymentRequest CreatePaymentRequest(
        string? cardNumber = null,
        int? expiryMonth = null,
        int? expiryYear = null,
        string? currency = null,
        int? amount = null,
        string? cvv = null)
    {
        return new PaymentRequest(
            cardNumber ?? GenerateNumberSequence(_cardLength),
            expiryMonth ?? GenerateExpiryMonth(),
            expiryYear ?? GenerateExpiryYear(),
            currency ?? GetCurrency(),
            amount ?? fixture.Create<int>(),
            cvv ?? GenerateNumberSequence(_cvvLength)
        );
    }
    
    public static PaymentResponseDao CreatePaymentResponseDao(
        bool? authorized = null,
        string? authorizationCode = null)
    {
        return new PaymentResponseDao(
            authorized ?? fixture.Create<bool>(),
            authorizationCode ?? (authorized == true ? fixture.Create<Guid>().ToString() : null)
        );
    }

    private static string GenerateNumberSequence(int length) =>
        string.Concat(Enumerable.Range(0, length).Select(_ => Math.Abs(fixture.Create<int>() % 10)));

    private static int GenerateExpiryMonth() =>
        Math.Abs(fixture.Create<int>() % 12 + 1);

    private static int GenerateExpiryYear() =>
        Math.Abs(fixture.Create<int>() % (2100 - DateTime.Now.Year + 1)) + DateTime.Now.Year;

    private static string GetCurrency() =>
        _approvedCurrencies[fixture.Create<int>() & int.MaxValue % _approvedCurrencies.Length];
}