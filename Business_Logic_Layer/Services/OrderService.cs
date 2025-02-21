using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Exception.Order;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{

    interface IOrderService
    {
        public Task<Order> CreateAsync();
        public Task<Order> EditAsync();
        public Task<IEnumerable<Order>> GetAllAsync();
        public Task<Order> GetByIdAsync(Guid orderId); 
        public Task<bool> DeleteByIdAsync(Guid orderId);
    }
    class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<Order> CreateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<Order> EditAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            var response = await _orderRepository.GetByIdAsync(orderId);
            if ( response!=null)
            {
                return response;
            }
            throw new OrderNotFoundException();
        }
    }


}
