using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
        Task<bool> VerifyZaloPayCallbackAsync(ZaloPayCallback callback);
    }
}
