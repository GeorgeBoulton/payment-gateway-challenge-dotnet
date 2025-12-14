using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Domain.Services;

namespace PaymentGateway.Domain;

public static class Startup
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentService, PaymentService>();
    }
}