using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendPaymentSuccessEmailAsync(Payment payment, Account account);
    }
}
