using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _paymentService.CreatePaymentAsync(request, HttpContext);
            return Ok(result);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var result = await _paymentService.ProcessPaymentCallbackAsync(Request.Query);

            // Redirect to a payment result page with the appropriate status
            return Redirect($"/payment/result?success={result.Success}&orderId={result.OrderId}");
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(Guid paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentsByOrder(Guid orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(payments);
        }
    }
}
