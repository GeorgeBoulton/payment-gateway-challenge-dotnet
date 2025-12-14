using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    IGetPaymentResponseMapper getPaymentResponseMapper,
    IPostPaymentResponseMapper postPaymentResponseMapper,
    IPaymentRequestMapper paymentRequestMapper
    ) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var response = paymentService.GetPayment(id);
        
        return Ok(getPaymentResponseMapper.Map(response));
    }
    
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var paymentRequest = paymentRequestMapper.Map(request);
        
        var response = await paymentService.ProcessPaymentAsync(paymentRequest);
        
        return Ok(postPaymentResponseMapper.Map(response));
    }
}