using Microsoft.Extensions.Logging;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.DAL.Processors;

// Makes sense to have both store and retrieve in here given size of project. Perhaps with larger project you may want
// to split up read and write.
public class PaymentDataProcessor(
    IPaymentsRepository paymentsRepository,
    IPaymentMapper paymentMapper,
    ILogger<PaymentDataProcessor> logger) : IPaymentDataProcessor
{
    public void StorePayment(Payment payment)
    {
        var paymentEntity = paymentMapper.Map(payment);
        paymentsRepository.Add(paymentEntity);
        
        logger.LogInformation("Payment stored with Id: {PaymentId}", paymentEntity.Id);
    }

    public Payment? RetrievePayment(Guid id)
    {
        var paymentEntity = paymentsRepository.Get(id);

        if (paymentEntity is not null)
        {
            return paymentMapper.Map(paymentEntity);
        }

        logger.LogInformation("No payment found matching Id: {PaymentId}", id);

        return null;
    }
}