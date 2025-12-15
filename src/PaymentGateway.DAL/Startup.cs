using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.DAL.Clients;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Processors;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.DAL;

public static class Startup
{
    public static void AddServices(this IServiceCollection services)
    {
        AddClients(services);
        AddRepositories(services);
        AddProcessors(services);
        AddMappers(services);
    }
    
    private static void AddClients(IServiceCollection services)
    {
        services.AddHttpClient<IBaseClient, BaseClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080");
        });
        services.AddSingleton<IBankSimulatorClient, BankSimulatorClient>();
    }
    
    private static void AddRepositories(IServiceCollection services)
    {
        // In this case we are just adding to a list with async method so makes sense to have singleton. If it were a
        // real repository it should absolutely not be singleton. Use scoped/transient to avoid concurrency problems.
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
    }
    
    private static void AddProcessors(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
        services.AddSingleton<IPaymentDataProcessor, PaymentDataProcessor>();
    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentMapper, PaymentMapper>();
        services.AddSingleton<IPaymentRequestDaoMapper, PaymentRequestDaoMapper>();
        services.AddSingleton<IPaymentResponseMapper, PaymentResponseMapper>();
    }
}