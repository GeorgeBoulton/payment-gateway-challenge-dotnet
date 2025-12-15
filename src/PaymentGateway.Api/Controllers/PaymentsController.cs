using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Domain.Services;
using PaymentGateway.Shared.Exceptions;

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
        
        if (response is null)
        {
            return NotFound();
        }
        
        return Ok(getPaymentResponseMapper.Map(response));
    }
    
    [HttpPost]
    public async Task<IActionResult> PostProcessPayment(PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var paymentRequest = paymentRequestMapper.Map(request);
        
        try
        {
            var response = await paymentService.ProcessPaymentAsync(paymentRequest);

            return Ok(postPaymentResponseMapper.Map(response));
        }
        catch (BankUnavailableException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { Message = "Bank simulator is currently unavailable. Please try again later." });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An unexpected error occurred. Please try again later." });
        }
    }
}