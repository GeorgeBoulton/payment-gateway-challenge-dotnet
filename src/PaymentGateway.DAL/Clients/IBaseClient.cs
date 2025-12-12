namespace PaymentGateway.DAL.Clients;

public interface IBaseClient
{
    Task<T> GetAsync<T>(Uri uri);
    Task<T> PostAsync<T>(Uri uri, HttpContent httpContent);
}