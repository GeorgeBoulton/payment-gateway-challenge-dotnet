using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public interface IPostPaymentResponseMapper
{
    PostPaymentResponse Map(Payment payment);
}