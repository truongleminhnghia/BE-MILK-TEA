using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public interface IOrderDetailService
    {
        public Task<OrderDetail> CreateAsync(OrderDetailRequest orderDetail);
   
        public Task<List<OrderDetail>> GetAllOrderDetailsAsync();
        public Task<OrderDetail> GetByIdAsync(Guid orderDetailId);
        public Task<bool> DeleteByIdAsync(Guid orderDetailId);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        public Task<OrderDetail> CreateAsync(OrderDetailRequest orderDetail)
        {
            
        }

        public async Task<bool> DeleteByIdAsync(Guid orderDetailId)
        {
            return await _orderDetailRepository.DeleteByIdAsync(orderDetailId);
        }
    
        public async Task<List<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return await _orderDetailRepository.GetAllOrdersDetailAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(Guid orderDetailId)
        {
            return await _orderDetailRepository.GetByIdAsync(orderDetailId);
        }
    }
}
