using PaymentGateway.DAL.Entities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.Mappers;

public class StatusMapper : IStatusMapper
{
    public PaymentStatus Map(Status status) =>
        status switch
        {
            Status.Authorized => PaymentStatus.Authorized,
            Status.Declined => PaymentStatus.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown status in DAL.")
        };
    
    public Status Map(PaymentStatus paymentStatus) =>
        paymentStatus switch
        {
            PaymentStatus.Authorized => Status.Authorized,
            PaymentStatus.Declined => Status.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentStatus), paymentStatus, $"Domain status {paymentStatus} not valid in DAL.")
        };
}