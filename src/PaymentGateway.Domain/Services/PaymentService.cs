using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Factories;
using PaymentGateway.Domain.Processors;

namespace PaymentGateway.Domain.Services;

public class PaymentService(
    IPaymentProcessor paymentProcessor,
    IPaymentDataProcessor paymentDataProcessor,
    IPaymentValidator paymentValidator,
    IPaymentFactory paymentFactory) : IPaymentService
{
    public Payment? GetPayment(Guid id)
    {
        var payment = paymentDataProcessor.RetrievePayment(id);

        return payment is not null ? MaskCardNumber(payment) : null; 
    }
    
    public async Task<Payment> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        var paymentId = Guid.NewGuid();
        
        if (!paymentValidator.IsPaymentValid(paymentRequest))
        {
            var rejectedPayment = paymentFactory.CreateRejected(paymentId, paymentRequest);
            return MaskCardNumber(rejectedPayment);
        }
        
        var paymentResponse = await paymentProcessor.ProcessPayment(paymentRequest);
        var payment = paymentFactory.CreateFromResponse(paymentId, paymentRequest, paymentResponse);
        
        paymentDataProcessor.StorePayment(payment);
        
        return MaskCardNumber(payment);
    }
    
    private static Payment MaskCardNumber(Payment payment)
    {
        return payment with { CardNumber = $"**** **** **** {payment.CardNumber[^4..]}" };
    }
}