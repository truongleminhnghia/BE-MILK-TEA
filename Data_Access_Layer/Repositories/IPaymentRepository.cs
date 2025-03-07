using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(Guid id);
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetByOrderIdAsync(Guid orderId);
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
    }
}
