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
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Services
{
    public interface IOrderService
    {
        public Task<OrderResponse> CreateAsync(OrderRequest order);
        //public Task<Order> EditAsync();
        public Task<List<OrderResponse>> GetAllAsync();
        public Task<OrderResponse> GetByIdAsync(Guid orderId); 
        public Task<bool> DeleteByIdAsync(Guid orderId);
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
            //Kiểm tra promotion code đã được sử dụng hay còn hạn không
            // double promotionPrice = 0.0;
            // Promotion promotion = _protionService.getByPromotionCode(orderRequest.PromotionCode);
            // if (promotion == null)
            // {
            //     //Bắt lỗi và trả về kết quả của promotion (là không tồn tại);
            // }
            //
            // if (promotion != null)
            // {
            //     if (promotion.IsActive == true && promotion.EndDate > DateTime.Now &&
            //         promotion.StartDate < DateTime.Now)
            //     {
            //         promotionPrice = ??
            //     }
            // }
            
            var order = _mapper.Map<Order>(orderRequest);
            order.OrderDate = DateTime.Now;
            // order.PriceAfterPromotion = order.TotalPrice - promotionPrice;
            var createdOrder= await _orderRepository.CreateAsync(order);
            return _mapper.Map<OrderResponse>(createdOrder);
        }

        public async Task<bool> DeleteByIdAsync(Guid orderId)
        {
            return await _orderRepository.DeleteByIdAsync(orderId);
        }

        //public Task<Order> EditAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return _mapper.Map<List<OrderResponse>>(orders);
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            return order==null ? null : _mapper.Map<OrderResponse>(order);
        }
    }


}
