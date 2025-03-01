using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.Identity.Client;


namespace Business_Logic_Layer.Services
{
    public interface IOrderService
    {
        public Task<OrderResponse> CreateAsync(OrderRequest order);
        public Task<List<OrderResponse>> GetAllAsync(Guid accountId, string? search, string? sortBy, bool isDescending,OrderStatus? orderStatus,DateTime? orderDate, int page, int pageSize);
        public Task<OrderResponse> GetByIdAsync(Guid orderId); 
        //public Task<bool> DeleteByIdAsync(Guid orderId);
     
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderDetailService _orderDetailService;
        private readonly CartRepository _cartRepository;
        private readonly CartItemService _cartItemService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<OrderResponse> CreateAsync(OrderRequest orderRequest)
        {
            try
            {
                var order = _mapper.Map<Order>(orderRequest);
                //order.OrderCode = Guid.NewGuid().ToString();
                order.OrderDate = DateTime.Now;
                // order.PriceAfterPromotion = order.TotalPrice - promotionPrice;
                var createdOrder = await _orderRepository.CreateAsync(order);
                return _mapper.Map<OrderResponse>(createdOrder);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo order", ex);
            }
        
        }

        //public async Task<bool> DeleteByIdAsync(Guid orderId)
        //{
        //    return await _orderRepository.DeleteByIdAsync(orderId);
        //}

        public async Task<List<OrderResponse>> GetAllAsync(Guid accountId,string? search, string? sortBy, bool isDescending, OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync(accountId, search, sortBy, isDescending, orderStatus, orderDate, page, pageSize);
                return _mapper.Map<List<OrderResponse>>(orders);        
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy list order", ex);
            }
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                return order == null ? null : _mapper.Map<OrderResponse>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin order bằng id", ex);
            }
        }
    }


}
