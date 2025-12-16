using System.Net;

using Microsoft.Extensions.Logging;

using PaymentGateway.DAL.DAOs;
using PaymentGateway.Shared.Exceptions;

namespace PaymentGateway.DAL.Clients;

public class BankSimulatorClient(
    IBaseClient baseClient,
    ILogger<BankSimulatorClient> logger) : IBankSimulatorClient
{
    private const string BaseUri = "http://localhost:8080";

    public async Task<PaymentResponseDao> ProcessPaymentRequestAsync(PaymentRequestDao paymentRequestDao)
    {
        try
        {
            var requestUri = new Uri(new Uri(BaseUri), "/payments");
            
            var postResponse = await baseClient.PostAsync<PaymentRequestDao, PaymentResponseDao>(requestUri, paymentRequestDao);
            logger.LogInformation("Payment processed by bank for card ending in: {Last4}", paymentRequestDao.CardNumber[^4..]);

            return postResponse;
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            logger.LogError(e, "{StatusCode}: Upstream service bank simulator unavailable. No payment processed for card ending in: {Last4}", e.StatusCode, paymentRequestDao.CardNumber[^4..]);
            throw new BankUnavailableException($"Bank simulator returned {e.StatusCode}.", e);
        }
    }
}