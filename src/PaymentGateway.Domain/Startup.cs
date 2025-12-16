using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Domain.Factories;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Domain;

public static class Startup
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IPaymentValidator, PaymentValidator>();
        services.AddSingleton<IPaymentFactory, PaymentFactory>();
    }
}