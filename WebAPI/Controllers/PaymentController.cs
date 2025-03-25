using System;
using System.Net;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.NotificationService;
using Business_Logic_Layer.Services.PaymentService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace WebAPI.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly INotificationService _emailService;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger,
            INotificationService emailService
        )
        {
            _paymentService = paymentService;
            _logger = logger;
            _emailService = emailService;
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
                    var errorMessages = string.Join(
                        ", ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    );
                    _logger.LogWarning($"Invalid model state: {errorMessages}");
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest.GetHashCode(),
                            false,
                            "Invalid model state",
                            errorMessages
                        )
                    );
                }

                var result = await _paymentService.CreatePaymentAsync(request, HttpContext);
                _logger.LogInformation(
                    $"Payment created successfully with PaymentId: {result.PaymentId}"
                );

                return Ok(
                    new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Payment created successfully",
                        result
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreatePayment: {ex.Message}");
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError.GetHashCode(),
                        false,
                        ex.Message,
                        null
                    )
                );
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

                if (payments == null || !payments.Any())
                {
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest.GetHashCode(),
                            false,
                            "No payments found for this OrderId",
                            null
                        )
                    );
                }

                return Ok(
                    new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Payments retrieved successfully",
                        payments
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPaymentsByOrder: {ex.Message}");
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError.GetHashCode(),
                        false,
                        ex.Message,
                        null
                    )
                );
            }
        }

        [HttpPost("payment-callback-url")]
        public async Task<IActionResult> PaymentCallbackUrl([FromBody] PaymentUrlRequest request)
        {
            try
            {
                _logger.LogInformation("Received VNPay callback URL: {Url}", request.FullUrl);

                // Chuyển đổi tham số URL sang IQueryCollection
                var uri = new Uri(request.FullUrl);
                var queryCollection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(
                    uri.Query
                );

                // Chuyển đổi IQueryCollection
                var queryDict = new QueryCollection(
                    queryCollection.ToDictionary(
                        k => k.Key,
                        v => new StringValues(v.Value.ToArray())
                    )
                );

                // Quá trình callback
                var response = await _paymentService.ProcessPaymentCallbackAsync(queryDict);

                var apiResponse = new ApiResponse(
                    response.Success ? 200 : 400,
                    response.Success,
                    response.Success
                        ? "Payment processed successfully"
                        : "Failed to process payment",
                    response
                );

                return StatusCode(apiResponse.Code, apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback.");

                var apiErrorResponse = new ApiResponse(
                    500,
                    false,
                    "Internal Server Error",
                    new { error = ex.Message }
                );

                return StatusCode(apiErrorResponse.Code, apiErrorResponse);
            }
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                // Create dummy payment and account data for testing
                var payment = new Payment
                {
                    AmountPaid = request.AmountPaid,
                    TranscationId = Guid.NewGuid().ToString(),
                };

                var account = new Account { Email = request.Email, FirstName = "Test User" };

                await _emailService.SendPaymentSuccessEmailAsync(payment, account);
                return Ok(new { message = $"✅ Test email sent to {request.Email}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Email sending failed: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send test email." });
            }
        }
    }
}
