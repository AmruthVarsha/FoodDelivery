using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Payment;
using OrderService.Application.Interfaces;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize(Roles = "Customer,Partner")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> SimulatePayment([FromBody] SimulatePaymentDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _paymentService.SimulatePaymentAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("{orderId:guid}/complete")]
        public async Task<IActionResult> CompletePayment(Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _paymentService.CompletePaymentAsync(orderId, userId);
            return Ok(result);
        }

        [HttpGet("{orderId:guid}/status")]
        public async Task<IActionResult> GetPaymentStatus(Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _paymentService.GetPaymentStatusAsync(orderId, userId);
            return Ok(result);
        }
    }
}
