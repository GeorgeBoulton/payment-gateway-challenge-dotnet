using PaymentGateway.Api.Mappers;

namespace PaymentGateway.Api;

public static class Startup
{
    public static void AddServices(this IServiceCollection services)
    {
        DAL.Startup.AddServices(services);
        Domain.Startup.AddServices(services);
        
        services.AddMappers();
    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentRequestMapper, PaymentRequestMapper>();
        services.AddSingleton<IGetPaymentResponseMapper, GetPaymentResponseMapper>();
        services.AddSingleton<IPostPaymentResponseMapper, PostPaymentResponseMapper>();
        services.AddSingleton<IPaymentStatusMapper, PaymentStatusMapper>();
    }

}