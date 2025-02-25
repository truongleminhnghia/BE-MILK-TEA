using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
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
        public Task<List<Order>> GetAllAsync();
        public Task<Order> GetByIdAsync(Guid orderId); 
        public Task<bool> DeleteByIdAsync(Guid orderId);
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderDetailService _orderDetailService;

        public Task<OrderResponse> CreateAsync(OrderRequest order)
        {
            //Tạo order
            Cart cart = _cartRepo.GetByIdAsync();
            List<CartItem> cartItemList = new List<CartItem>();
            foreach (CartDetailDTO cartDetailDTO : order.cartDetailList) {
                CartItem cartItem = _cartItemService.GetById();
                cartItemList.Add(cartItem);
            }
            Order order = new Order();
            foreach (CartItem item in cartItemList) {
                OrderDetail orderDetail = _orderDetailService.CreateAsync(item);
                order.OrderDetails.Add(orderDetail);
            }
            _orderService.SaveOrder(order);

            //Trả response
            OrderResponse orderResponse = new OrderResponse();

            //Thêm thông tin vào orderResponse
            foreach (OrderDetail orderDetail in order.OrderDetails) {
                OrderDetailResponse orderDetailResponse = new OrderDetailResponse();
                //Thêm thông tin vào OrderDetailResponse 
                orderDetailResponse.IngredientProductId = orderDetail.IngredientProductId();

                //Nhét OrderDetailResponse vào trong OrderResponse
                orderResponse.orderDetailResponses.add(orderDetailResponse);
            }
            

            //Trả về kết quả
            return orderResponse;
        }

        public async Task<bool> DeleteByIdAsync(Guid orderId)
        {
            return await _orderRepository.DeleteByIdAsync(orderId);
        }

        //public Task<Order> EditAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }
    }


}
