using PaymentGateway.DAL.DAOs;

namespace PaymentGateway.DAL.Clients;

public interface IBankSimulatorClient
{
    Task<PaymentResponseDao> ProcessPaymentRequestAsync(PaymentRequestDao paymentRequestDao);
}