using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync(Guid accountId,string? search, string? sortBy, bool isDescending,OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize);
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(Order order);
        Task<Order?> UpdateStatusAsync(Guid id, Order order);
        //Task<bool> DeleteByIdAsync(Guid id);
    }
}
