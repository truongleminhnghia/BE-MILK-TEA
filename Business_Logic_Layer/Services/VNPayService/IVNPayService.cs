using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace Business_Logic_Layer.Services.VNPayService
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(PaymentCreateRequest request, HttpContext httpContext);
        PaymentResponse ProcessPaymentCallback(IQueryCollection collections);
    }
}
