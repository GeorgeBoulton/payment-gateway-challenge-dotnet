using System.Net;
using System.Text;
using System.Text.Json;

using PaymentGateway.Application.Exceptions;
using PaymentGateway.DAL.DAOs;

namespace PaymentGateway.DAL.Clients;

public class BankSimulatorClient(IBaseClient baseClient)
{
    private const string BaseUri = "http://localhost:8080";

    public async Task<PaymentResponseDao> ProcessPaymentRequestAsync(PaymentRequestDao paymentRequestDao)
    {
        try
        {
            var requestUri = new Uri(new Uri(BaseUri), "/payments");
            return await MakeRequest<PaymentResponseDao>(requestUri, JsonSerializer.Serialize(paymentRequestDao));

        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            throw new BankUnavailableException("Bank simulator returned 503.", e);
        }
    }

    private async Task<T> MakeRequest<T>(Uri requestUri, string requestBody)
    {
        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        return await baseClient.PostAsync<T>(requestUri, content);
    }
}