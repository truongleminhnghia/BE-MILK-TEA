using System;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger
        )
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateRequest request)
        {
            try
            {
                _logger.LogInformation(
                    $"CreatePayment endpoint called with OrderId: {request.OrderId}"
                );

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        $"Invalid model state: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}"
                    );
                    return BadRequest(ModelState);
                }

                var result = await _paymentService.CreatePaymentAsync(request, HttpContext);
                _logger.LogInformation(
                    $"Payment created successfully with PaymentId: {result.PaymentId}"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreatePayment: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("orderId")]
        public async Task<IActionResult> GetPaymentsByOrder(Guid orderId)
        {
            try
            {
                _logger.LogInformation(
                    $"GetPaymentsByOrder endpoint called for OrderId: {orderId}"
                );

                var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPaymentsByOrder: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
