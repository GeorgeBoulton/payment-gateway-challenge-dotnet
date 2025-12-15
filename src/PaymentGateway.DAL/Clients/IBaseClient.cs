namespace PaymentGateway.DAL.Clients;

public interface IBaseClient
{
    Task<T> GetAsync<T>(Uri uri);
    Task<TResponse> PostAsync<TRequest, TResponse>(Uri uri, TRequest request);
}