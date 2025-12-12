using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentAsync(Guid id)
    {

        return payment is null ? NotFound() : Ok(payment);
    }
    
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await paymentService.ProcessPaymentAsync(request);
        
        return Ok(response);
    }
}