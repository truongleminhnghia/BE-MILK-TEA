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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Business_Logic_Layer.Services.VNPayService
{
    public class VNPayService : IVNPayService
    {
        private readonly VNPayConfiguration _vnPayConfig;
        private readonly ILogger<VNPayService> _logger;

        public VNPayService(IOptions<VNPayConfiguration> vnPayConfig, ILogger<VNPayService> logger)
        {
            _vnPayConfig = vnPayConfig.Value;
            _logger = logger;
        }

        public string CreatePaymentUrl(PaymentCreateRequest request, HttpContext httpContext, double totalPrice)
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
            vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString()); // Convert to VND (smallest unit)
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
            _logger.LogInformation(
                "Processing VNPay callback: {Collections}",
                string.Join(", ", collections.Select(c => $"{c.Key}={c.Value}"))
            );

            var response = new PaymentResponse();

            // Check if collections contains necessary data
            if (collections.Count > 0)
            {
                string vnp_ResponseCode = collections["vnp_ResponseCode"];
                string vnp_TransactionStatus = collections["vnp_TransactionStatus"];
                string vnp_TxnRef = collections["vnp_TxnRef"];
                string vnp_Amount = collections["vnp_Amount"];
                string vnp_TransactionNo = collections["vnp_TransactionNo"];

                _logger.LogInformation(
                    "VNPay callback details: ResponseCode={ResponseCode}, TransactionStatus={TransactionStatus}, TxnRef={TxnRef}",
                    vnp_ResponseCode,
                    vnp_TransactionStatus,
                    vnp_TxnRef
                );

                // Parse OrderId from vnp_TxnRef
                if (Guid.TryParse(vnp_TxnRef, out Guid orderId))
                {
                    response.OrderId = orderId;
                    _logger.LogInformation("Successfully parsed OrderId: {OrderId}", orderId);
                }
                else
                {
                    _logger.LogError(
                        "Failed to parse OrderId from vnp_TxnRef: {TxnRef}",
                        vnp_TxnRef
                    );
                    response.Success = false;
                    response.Message = "Invalid order reference";
                    return response;
                }

                // Parse Amount from vnp_Amount (note: VNPay amount is in VND with last 2 digits removed)
                if (long.TryParse(vnp_Amount, out long amountVnd))
                {
                    // VNPay sends amount * 100, so divide by 100 to get actual amount
                    double amount = (double)amountVnd / 100;
                    response.Amount = amount;
                    _logger.LogInformation("Successfully parsed Amount: {Amount}", amount);
                }
                else
                {
                    _logger.LogError(
                        "Failed to parse Amount from vnp_Amount: {Amount}",
                        vnp_Amount
                    );
                }

                // Parse Transaction ID
                response.TransactionId = vnp_TransactionNo;

                // Check if payment was successful
                // VNPay success codes are "00" for both ResponseCode and TransactionStatus
                bool isSuccess = vnp_ResponseCode == "00" && vnp_TransactionStatus == "00";

                response.Success = isSuccess;
                response.Message = isSuccess ? "Payment successful" : "Payment failed";

                _logger.LogInformation(
                    "VNPay payment {Status}: {Message}",
                    isSuccess ? "successful" : "failed",
                    response.Message
                );
            }
            else
            {
                _logger.LogError("Empty VNPay callback collections");
                response.Success = false;
                response.Message = "No VNPay response data";
            }

            return response;
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
