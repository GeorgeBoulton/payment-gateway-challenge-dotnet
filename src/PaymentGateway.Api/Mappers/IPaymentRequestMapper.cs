using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Api.Mappers;

public interface IPaymentRequestMapper
{
    PaymentRequest Map(PostPaymentRequest postPaymentRequest);
}