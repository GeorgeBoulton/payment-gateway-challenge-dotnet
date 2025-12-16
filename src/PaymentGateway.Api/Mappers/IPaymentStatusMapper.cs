using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Mappers;

public interface IPaymentStatusMapper
{
    PaymentStatus Map(Domain.Entities.PaymentStatus paymentStatus);
}