using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services.EmailService;
using Business_Logic_Layer.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public PaymentController(IPaymentService paymentService, IEmailService emailService)
        {
            _paymentService = paymentService;
            _emailService = emailService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.CreatePaymentAsync(request);

            // Gửi email sau khi thanh toán thành công
            if (result.ReturnCode == 1)
            {
                _emailService.SendEmailAsync(
                    request.Email,
                    "Thanh toán thành công",
                    "Cảm ơn bạn đã thanh toán!"
                );
            }

            return Ok(result);
        }

        [HttpPost("zalopay-callback")]
        public async Task<IActionResult> ZaloPayCallback([FromBody] ZaloPayCallback callback)
        {
            bool success = await _paymentService.VerifyZaloPayCallbackAsync(callback);
            return success
                ? Ok(new { message = "Thanh toán đã được xác minh" })
                : BadRequest(new { message = "Thanh Toán Thất Bại" });
        }
    }
}
