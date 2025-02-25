using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<List<OrderDetail>> GetAllOrdersDetailAsync();
        Task<OrderDetail?> GetByIdAsync(Guid id);
        Task<OrderDetail> CreateAsync(OrderDetail orderDetail);
        Task<OrderDetail?> UpdateAsync(Guid id, OrderDetail orderDetail);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
