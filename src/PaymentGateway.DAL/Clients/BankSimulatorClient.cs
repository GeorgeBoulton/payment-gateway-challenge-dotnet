using System.Net;
using PaymentGateway.DAL.DAOs;
using PaymentGateway.Shared.Exceptions;

namespace PaymentGateway.DAL.Clients;

public class BankSimulatorClient(IBaseClient baseClient) : IBankSimulatorClient
{
    private const string BaseUri = "http://localhost:8080";

    public async Task<PaymentResponseDao> ProcessPaymentRequestAsync(PaymentRequestDao paymentRequestDao)
    {
        try
        {
            var requestUri = new Uri(new Uri(BaseUri), "/payments");
            
            return await baseClient.PostAsync<PaymentRequestDao, PaymentResponseDao>(requestUri, paymentRequestDao);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            throw new BankUnavailableException("Bank simulator returned 503.", e);
        }
    }
}