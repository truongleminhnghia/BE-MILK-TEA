using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Configurations;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Business_Logic_Layer.Services.VNPayService
{
    public class VNPayService : IVNPayService
    {
        private readonly VNPayConfiguration _vnPayConfig;

        public VNPayService(IOptions<VNPayConfiguration> vnPayConfig)
        {
            _vnPayConfig = vnPayConfig.Value;
        }

        public string CreatePaymentUrl(PaymentCreateRequest request, HttpContext httpContext)
        {
            string version = Environment.GetEnvironmentVariable("Version");
            string tmnCode = Environment.GetEnvironmentVariable("TmnCode");
            string hashSecret = Environment.GetEnvironmentVariable("HashSecret");
            string baseUrl = Environment.GetEnvironmentVariable("BaseUrl");
            string command = Environment.GetEnvironmentVariable("Command");
            string currCode = Environment.GetEnvironmentVariable("CurrCode");
            string locale = Environment.GetEnvironmentVariable("Locale");
            string paymentBackReturnUrl = Environment.GetEnvironmentVariable(
                "PaymentBackReturnUrl"
            );
            string timeZoneId = Environment.GetEnvironmentVariable("SE Asia Standard Time");

            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_Version", version);
            vnpay.AddRequestData("vnp_Command", command);
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", (request.TotalPrice * 100).ToString()); // Convert to VND (smallest unit)
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", currCode);
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", locale);
            vnpay.AddRequestData(
                "vnp_OrderInfo",
                request.OrderDescription ?? _vnPayConfig.OrderInfo
            );
            vnpay.AddRequestData("vnp_OrderType", "other"); // Order type
            vnpay.AddRequestData("vnp_ReturnUrl", paymentBackReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", request.OrderId.ToString()); // Your transaction ID

            string paymentUrl = vnpay.CreateRequestUrl(baseUrl, hashSecret);
            return paymentUrl;
        }

        public PaymentResponse ProcessPaymentCallback(IQueryCollection collections)
        {
            var vnpay = new VNPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            string orderId = vnpay.GetResponseData("vnp_TxnRef");
            string vnPayTransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");

            bool isValidSignature = vnpay.ValidateSignature(
                vnpay.GetResponseData("vnp_SecureHash"),
                _vnPayConfig.HashSecret
            );

            if (isValidSignature)
            {
                var response = new PaymentResponse
                {
                    OrderId = Guid.Parse(orderId),
                    TransactionId = vnPayTransactionId,
                    Success = responseCode == "00",
                    PaymentMethod = PaymentMethod.VNPAY,
                    Amount = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount")) / 100, // Convert from smallest unit
                    PaymentDate = DateTime.ParseExact(
                        vnpay.GetResponseData("vnp_PayDate"),
                        "yyyyMMddHHmmss",
                        CultureInfo.InvariantCulture
                    ),
                };

                return response;
            }

            return new PaymentResponse { Success = false, Message = "Invalid signature" };
        }

        private string GetIpAddress(HttpContext httpContext)
        {
            string ipAddress;
            try
            {
                ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = "127.0.0.1";
                }
            }
            catch (Exception)
            {
                ipAddress = "127.0.0.1";
            }
            return ipAddress;
        }
    }
}
