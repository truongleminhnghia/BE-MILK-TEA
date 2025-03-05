using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.NotificationService;
using Business_Logic_Layer.Services.VNPayService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Business_Logic_Layer.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVNPayService _vnPayService;
        private readonly INotificationService _notificationService;
        private readonly IAccountRepository _accountRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IVNPayService vnPayService,
            INotificationService notificationService,
            IAccountRepository accountRepository
        )
        {
            _paymentRepository = paymentRepository;
            _vnPayService = vnPayService;
            _notificationService = notificationService;
            _accountRepository = accountRepository;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(
            PaymentCreateRequest request,
            HttpContext httpContext
        )
        {
            // Create payment record in pending status
            var payment = new Payment
            {
                OrderId = request.OrderId,
                PaymentMethod = PaymentMethod.VNPAY,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.Pending,
                TotlaPrice = request.TotalPrice, // Note the typo in your entity
                AmountPaid = 0,
                RemainingAmount = request.TotalPrice,
            };

            await _paymentRepository.CreateAsync(payment);

            // Generate payment URL
            string paymentUrl = _vnPayService.CreatePaymentUrl(request, httpContext);

            return new PaymentResponse
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                PaymentUrl = paymentUrl,
                Success = true,
                Message = "URL thanh toán được tạo thành công",
            };
        }

        public async Task<PaymentResponse> ProcessPaymentCallbackAsync(IQueryCollection collections)
        {
            var response = _vnPayService.ProcessPaymentCallback(collections);
            if (response.Success)
            {
                // Find the pending payment for this order
                var payments = await _paymentRepository.GetByOrderIdAsync(response.OrderId);
                var pendingPayment = payments.FirstOrDefault(p =>
                    p.PaymentStatus == PaymentStatus.Pending
                );

                if (pendingPayment != null)
                {
                    // Update payment status
                    pendingPayment.PaymentStatus = PaymentStatus.SUCCESS;
                    pendingPayment.TranscationId = response.TransactionId;
                    pendingPayment.AmountPaid = response.Amount;
                    pendingPayment.RemainingAmount = pendingPayment.TotlaPrice - response.Amount;
                    await _paymentRepository.UpdateAsync(pendingPayment);

                    // Retrieve the account associated with the order
                    var account = await _accountRepository.GetById(response.OrderId);

                    // Send payment success notification
                    if (account != null)
                    {
                        await _notificationService.SendPaymentSuccessEmailAsync(
                            pendingPayment,
                            account
                        );
                    }

                    response.PaymentId = pendingPayment.Id;
                    response.Message = "Payment completed successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No pending payment found for this order";
                }
            }
            return response;
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid paymentId)
        {
            return await _paymentRepository.GetByIdAsync(paymentId);
        }

        public async Task<List<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _paymentRepository.GetByOrderIdAsync(orderId);
        }
    }
}
