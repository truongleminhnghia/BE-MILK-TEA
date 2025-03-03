using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Business_Logic_Layer.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IConfiguration config,
            HttpClient httpClient
        )
        {
            _paymentRepository = paymentRepository;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            var orderId = Guid.NewGuid();
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                PaymentMethod = request.PaymentMethod,
                PaymentDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Pending,
                TotlaPrice = request.TotalPrice,
                AmountPaid = 0,
                RemainingAmount = request.TotalPrice,
            };

            await _paymentRepository.CreatePaymentAsync(payment);

            // Gửi request đến ZaloPay
            var paymentRequest = new
            {
                app_id = _config["ZaloPay:AppId"],
                app_user = "demo_user",
                app_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                amount = request.TotalPrice,
                embed_data = JsonConvert.SerializeObject(new { order_id = orderId }),
                callback_url = _config["ZaloPay:CallbackUrl"],
                item = "[]",
            };

            var response = await _httpClient.PostAsync(
                "https://sandbox.zalopay.vn/v2/create",
                new StringContent(
                    JsonConvert.SerializeObject(paymentRequest),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<PaymentResponse>(result);

            return jsonResponse;
        }

        public async Task<bool> VerifyZaloPayCallbackAsync(ZaloPayCallback callback)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(
                Guid.Parse(callback.OrderId)
            );
            if (payment != null)
            {
                payment.PaymentStatus =
                    callback.Status == 1 ? PaymentStatus.SUCCESS : PaymentStatus.FAILED;
                payment.AmountPaid = callback.Amount;
                payment.RemainingAmount = payment.TotlaPrice - callback.Amount;
                await _paymentRepository.UpdatePaymentStatusAsync(
                    payment.Id,
                    payment.PaymentStatus
                );

                return payment.PaymentStatus == PaymentStatus.SUCCESS;
            }
            return false;
        }
    }
}
