using PaymentGateway.DAL.Clients;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.DAL.Processors;

public class PaymentProcessor(
    IBankSimulatorClient bankSimulatorClient,
    IPaymentRequestDaoMapper paymentRequestDaoMapper,
    IPaymentResponseMapper paymentResponseMapper) : IPaymentProcessor
{
    public async Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
    {
        var paymentRequestDao = paymentRequestDaoMapper.Map(paymentRequest);
        var paymentResponseDao = await bankSimulatorClient.ProcessPaymentRequestAsync(paymentRequestDao);

        return paymentResponseMapper.Map(paymentResponseDao);
    }
}