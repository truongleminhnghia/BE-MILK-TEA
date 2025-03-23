using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Services
{
    public interface IOrderDetailService
    {
        public Task<OrderDetail> CreateAsync(OrderDetail orderDetail);
        public Task<List<OrderDetailResponse>> GetAllOrderDetailsAsync(Guid orderId, string? search, string? sortBy, bool isDescending, int page, int pageSize);
        public Task<OrderDetail?> GetByIdAsync(Guid orderDetailId);
        public Task<OrderDetail?> UpdateAsync(Guid id, OrderDetail orderDetail);
        public Task<bool> DeleteByIdAsync(Guid orderDetailId);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICartItemRepository _cartItemRepository;
        //private readonly IIngredientProductRepository _ingredientProductRepository;
        private readonly IMapper _mapper;
        public OrderDetailService(IOrderDetailRepository orderDetailRepository,IMapper mapper, ICartItemRepository cartItemRepository, IIngredientRepository ingredientRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _cartItemRepository = cartItemRepository;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<bool> DeleteByIdAsync(Guid orderDetailId)
        {
            try
            {
                return await _orderDetailRepository.DeleteByIdAsync(orderDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể xóa chi tiết đơn hàng với ID: {orderDetailId}", ex);
            }
        }

        public async Task<OrderDetail> CreateAsync(OrderDetail orderDetail)
        {
            try
            {
                if (orderDetail == null)
                {
                    throw new ArgumentNullException(nameof(orderDetail), "không có thông tin OrderDetail ");
                }

                // Fetch the ingredient product asynchronously
                var cartItem = await _cartItemRepository.GetById(orderDetail.CartItemId);
                var ingredientProduct = await _ingredientRepository.GetById(cartItem.IngredientId);
                orderDetail.Quantity = cartItem.Quantity;
                orderDetail.Price = cartItem.Price;
                // Ensure ingredientProduct is not null
                if (ingredientProduct == null)
                {
                    throw new Exception($"không tìm được nguyen liệu với id{cartItem.IngredientId}");
                }
                //orderDetail.Quantity = _ingredientProductRepository.GetIngredientProductbyId(orderDetail.IngredientProductId).Result.Quantity;
                return await _orderDetailRepository.CreateAsync(orderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo chi tiết đơn hàng", ex);
            }
        }

      public async Task<List<OrderDetailResponse>> GetAllOrderDetailsAsync(Guid orderId, string? search, string? sortBy, bool isDescending, int page, int pageSize)
        {
            try
            {
                var orderDetails = await _orderDetailRepository.GetAllOrdersDetailAsync(orderId, search, sortBy, isDescending, page, pageSize);
                return _mapper.Map<List<OrderDetailResponse>>(orderDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách chi tiết đơn hàng", ex);
            }
        }

        public async Task<OrderDetail?> GetByIdAsync(Guid orderDetailId)
        {
            try
            {
                return await _orderDetailRepository.GetByIdAsync(orderDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể lấy chi tiết đơn hàng với ID: {orderDetailId}", ex);
            }
        }

        public async Task<OrderDetail?> UpdateAsync(Guid id, OrderDetail orderDetail)
        {
            try
            {
                var existingOrderDetail = await _orderDetailRepository.GetByIdAsync(id);
                if (existingOrderDetail == null)
                {
                    return null;
                }

                return await _orderDetailRepository.UpdateAsync(id, orderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể cập nhật chi tiết đơn hàng với ID: {id}", ex);
            }
        }
    }
}
