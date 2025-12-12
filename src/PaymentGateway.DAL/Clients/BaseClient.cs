using System.Text.Json;

namespace PaymentGateway.DAL.Clients;

public class BaseClient(HttpClient httpClient) : IBaseClient
{
    public async Task<T> GetAsync<T>(Uri uri)
    {
        return await SendAsync<T>(HttpMethod.Get, uri);
    }

    public async Task<T> PostAsync<T>(Uri uri, HttpContent httpContent)
    {
        return await SendAsync<T>(HttpMethod.Post, uri, httpContent);
    }

    private async Task<T> SendAsync<T>(HttpMethod httpMethod, Uri uri, HttpContent? httpContent = null)
    {
        var request = new HttpRequestMessage(httpMethod, uri);
        
        if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
        {
            request.Content = httpContent ??
                              throw new ArgumentException("Content must be provided for POST and PUT requests");
        }
        
        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        // Handle non-success status codes as needed
        throw new HttpRequestException($"Request to {uri} failed with status code {response.StatusCode}");
    }
}