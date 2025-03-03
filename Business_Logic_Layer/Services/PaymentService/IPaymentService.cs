using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;

namespace Business_Logic_Layer.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(
            PaymentCreateRequest request,
            HttpContext httpContext
        );
        Task<PaymentResponse> ProcessPaymentCallbackAsync(IQueryCollection collections);
        Task<Payment> GetPaymentByIdAsync(Guid paymentId);
        Task<List<Payment>> GetPaymentsByOrderIdAsync(Guid orderId);
    }
}
