using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.NotificationService;
using Business_Logic_Layer.Services.VNPayService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Business_Logic_Layer.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVNPayService _vnPayService;
        private readonly INotificationService _notificationService;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IVNPayService vnPayService,
            INotificationService notificationService,
            IAccountRepository accountRepository,
            IOrderRepository orderRepository,
            ILogger<PaymentService> logger
        )
        {
            _paymentRepository = paymentRepository;
            _vnPayService = vnPayService;
            _notificationService = notificationService;
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(
            PaymentCreateRequest request,
            HttpContext httpContext
        )
        {
            try
            {
                _logger.LogInformation(
                    "Tạo thanh toán cho OrderId: {OrderId}, Amount: {Amount}",
                    request.OrderId,
                    request.TotalPrice
                );

                // Create payment record in pending status
                var payment = new Payment
                {
                    OrderId = request.OrderId,
                    PaymentMethod = PaymentMethod.VNPAY,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = PaymentStatus.Pending,
                    TotlaPrice = request.TotalPrice,
                    AmountPaid = 0,
                    RemainingAmount = request.TotalPrice,
                };

                await _paymentRepository.CreateAsync(payment);
                _logger.LogInformation(
                    "Hồ sơ thanh toán được tạo bằng Id: {PaymentId}",
                    payment.Id
                );

                // Generate payment URL
                string paymentUrl = _vnPayService.CreatePaymentUrl(request, httpContext);
                _logger.LogInformation("URL thanh toán được tạo: {PaymentUrl}", paymentUrl);

                return new PaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    PaymentUrl = paymentUrl,
                    Success = true,
                    Message = "URL thanh toán được tạo thành công",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thanh toán");
                throw;
            }
        }

        public async Task<PaymentResponse> ProcessPaymentCallbackAsync(IQueryCollection collections)
        {
            try
            {
                // Log entire query string for debugging
                _logger.LogInformation(
                    "Processing payment callback: {QueryString}",
                    string.Join("&", collections.Select(c => $"{c.Key}={c.Value}"))
                );

                // Get response from VNPay service
                var response = _vnPayService.ProcessPaymentCallback(collections);
                _logger.LogInformation(
                    "VNPay response parsed: Success={Success}, OrderId={OrderId}, TransactionId={TransactionId}, Amount={Amount}",
                    response.Success,
                    response.OrderId,
                    response.TransactionId,
                    response.Amount
                );

                if (response.Success)
                {
                    // Find the pending payment for this order
                    var payments = await _paymentRepository.GetByOrderIdAsync(response.OrderId);
                    _logger.LogInformation(
                        "Found {Count} payments for OrderId: {OrderId}",
                        payments.Count,
                        response.OrderId
                    );

                    var pendingPayment = payments.FirstOrDefault(p =>
                        p.PaymentStatus == PaymentStatus.Pending
                    );

                    if (pendingPayment != null)
                    {
                        _logger.LogInformation(
                            "Đã tìm thấy khoản thanh toán đang chờ xử lý với Id: {PaymentId}, current status: {Status}",
                            pendingPayment.Id,
                            pendingPayment.PaymentStatus
                        );

                        try
                        {
                            // Cập nhật trạng thái thanh toán
                            pendingPayment.PaymentStatus = PaymentStatus.Success;
                            pendingPayment.TranscationId = response.TransactionId;
                            pendingPayment.AmountPaid = response.Amount;
                            pendingPayment.RemainingAmount =
                                pendingPayment.TotlaPrice - response.Amount;

                            _logger.LogInformation(
                                "Đang cập nhật thanh toán with: Status={Status}, TransactionId={TransactionId}, AmountPaid={AmountPaid}",
                                pendingPayment.PaymentStatus,
                                pendingPayment.TranscationId,
                                pendingPayment.AmountPaid
                            );

                            // Gỡ lỗi: Kiểm tra giá trị trạng thái
                            var statusValue = (int)pendingPayment.PaymentStatus;
                            _logger.LogInformation(
                                "Giá trị enum trạng thái thanh toán = {StatusValue}",
                                statusValue
                            );

                            // Cập nhật thanh toán trong cơ sở dữ liệu
                            var updatedPayment = await _paymentRepository.UpdateAsync(
                                pendingPayment
                            );
                            _logger.LogInformation(
                                "Kết quả cập nhật thanh toán: Status={Status}",
                                updatedPayment.PaymentStatus
                            );

                            // Kiểm tra lại xem bản cập nhật đã thành công chưa
                            var verifyPayment = await _paymentRepository.GetByIdAsync(
                                pendingPayment.Id
                            );
                            _logger.LogInformation(
                                "Trạng thái thanh toán đã được xác minh sau khi cập nhật: {Status}",
                                verifyPayment?.PaymentStatus.ToString() ?? "null"
                            );
                            _logger.LogInformation(
                                "Trước khi cập nhật - Trạng thái thanh toán: {Status}, Status enum value: {EnumValue}",
                                pendingPayment.PaymentStatus,
                                (int)pendingPayment.PaymentStatus
                            );

                            // Lấy lệnh tìm tài khoản liên quan
                            var order = await _orderRepository.GetByIdAsync(response.OrderId);

                            if (order != null && order.AccountId != Guid.Empty)
                            {
                                // Lấy lại tài khoản liên quan đến đơn hàng
                                var account = await _accountRepository.GetById(order.AccountId);

                                // Gửi thông báo thanh toán thành công
                                if (account != null)
                                {
                                    await _notificationService.SendPaymentSuccessEmailAsync(
                                        pendingPayment,
                                        account
                                    );
                                }
                            }

                            response.PaymentId = pendingPayment.Id;
                            response.Message = "Thanh toán hoàn tất thành công";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Lỗi khi cập nhật trạng thái thanh toán cho: {PaymentId}",
                                pendingPayment.Id
                            );
                            response.Success = false;
                            response.Message = $"Lỗi cập nhật thanh toán: {ex.Message}";
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Không tìm thấy khoản thanh toán đang chờ xử lý nào OrderId: {OrderId}",
                            response.OrderId
                        );
                        response.Success = false;
                        response.Message =
                            "Không tìm thấy khoản thanh toán đang chờ xử lý nào cho đơn hàng này";
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Lỗi xử lý thanh toán: {ex.Message}",
                };
            }
        }

        public async Task<List<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _paymentRepository.GetByOrderIdAsync(orderId);
        }
    }
}
