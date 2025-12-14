using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.DAL.Repositories;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.DAL.Processors;

// Makes sense to have both store and retrieve in here given size of project. Perhaps with larger project you may want
// to split up read and write.
public class PaymentDataProcessor(
    IPaymentsRepository paymentsRepository,
    IPaymentMapper paymentMapper) : IPaymentDataProcessor
{
    public void StorePayment(Payment payment)
    {
        var paymentEntity = paymentMapper.Map(payment);
        paymentsRepository.Add(paymentEntity);
    }

    public Payment RetrievePayment(Guid id)
    {
        var paymentEntity = paymentsRepository.Get(id);

        var payment = paymentMapper.Map(paymentEntity);

        return payment with { CardNumber = $"**** **** **** {payment.CardNumber[^4..]}" };
    }
}