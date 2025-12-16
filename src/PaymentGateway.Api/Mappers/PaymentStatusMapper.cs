using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Mappers;

public class PaymentStatusMapper : IPaymentStatusMapper
{
    public PaymentStatus Map(Domain.Entities.PaymentStatus paymentStatus) =>
        paymentStatus switch
        {
            Domain.Entities.PaymentStatus.Authorized => PaymentStatus.Authorized,
            Domain.Entities.PaymentStatus.Declined => PaymentStatus.Declined,
            Domain.Entities.PaymentStatus.Rejected => PaymentStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentStatus), paymentStatus, "Unknown payment status")
        };
        // Although this exception will never be thrown in our case, if we add more Status options in the future this is
        // for safety
}