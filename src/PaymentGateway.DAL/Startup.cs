using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.DAL.Clients;

namespace PaymentGateway.DAL;

public static class Startup
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<IBaseClient, BaseClient>(client =>
        {
            //client.BaseAddress = new Uri(Configuration["BaseUrl"]); todo implement config
        });
        //.AddPolicyHandler(GetRetryPolicy())
        //.AddPolicyHandler(GetCircuitBreakerPolicy());
    }
}