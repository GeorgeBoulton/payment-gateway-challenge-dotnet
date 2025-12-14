using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public interface IGetPaymentResponseMapper
{
    GetPaymentResponse Map(Payment payment);
}