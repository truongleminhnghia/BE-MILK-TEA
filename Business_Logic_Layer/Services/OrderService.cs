using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.Identity.Client;



namespace Business_Logic_Layer.Services
{
    public interface IOrderService
    {
        public Task<OrderResponse> CreateAsync(OrderRequest order);
        public Task<List<OrderResponse>> GetAllAsync(Guid accountId, string? search, string? sortBy, bool isDescending, OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize);
        public Task<OrderResponse> GetByIdAsync(Guid orderId);
        public Task<Order> Update(Order orderId);
        public Task<Order> UpdateStatus(Guid id, Order order);
        //public Task<bool> DeleteByIdAsync(Guid orderId);

    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IIngredientProductService _ingredientProductService;
        //private readonly IIngredientService _ingredientService;
        private readonly CartRepository _cartRepository;
        private readonly CartItemService _cartItemService;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IOrderDetailService orderDetailService, Source source)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderDetailService = orderDetailService;
            _source = source;
        }
        public async Task<OrderResponse> CreateAsync(OrderRequest orderRequest)
        {
            try
            {
                // var source = new Source();
                var order = _mapper.Map<Order>(orderRequest);
                order.OrderCode = "OD" + _source.GenerateRandom8Digits();
                order.OrderDate = DateTime.Now;
                order.OrderStatus= OrderStatus.PENDING_CONFIRMATION;
                //order.PriceAfterPromotion = order.TotalPrice - promotionPrice;
                var createdOrder = await _orderRepository.CreateAsync(order);
                List<OrderDetail> orderDetailList = new List<OrderDetail>();

                foreach (OrderDetailRequest orderDetail in orderRequest.orderDetailList)
                {
                    var orderDetails = new OrderDetail();
                    //var ingredients = new Ingredient();
                    var ingredientProduct = new IngredientProduct();

                    //xử lý quantity
                    //var chosenIngredientProduct = await _ingredientProductService.GetIngredientProductbyId(orderDetail.IngredientProductId);
                    //if (orderDetail.Quantity > chosenIngredientProduct.Quantity)
                    //{
                    //    throw new Exception($"Số lượng đặt hàng ({orderDetail.Quantity}) vượt quá số lượng sẵn có ({chosenIngredientProduct.Quantity}) của sản phẩm {chosenIngredientProduct.IngredientId}");
                    //}
                    //chosenIngredientProduct.Quantity -= order.Quantity;
                    //await _ingredientProductService.UpdateIngredientProduct(chosenIngredientProduct);
                    //chosenProduct.QuantityPerCarton -= createdOrder.Quantity;

                    
                    //tạo orderdetail
                    orderDetails.OrderId = createdOrder.Id;
                    orderDetails.IngredientProductId = orderDetail.IngredientProductId;
                    orderDetails.Quantity = orderDetail.Quantity;
                    var createOrderDetail = await _orderDetailService.CreateAsync(orderDetails);
                    orderDetailList.Add(createOrderDetail);
                    createdOrder.Quantity += createOrderDetail.Quantity;
                    createdOrder.TotalPrice += createOrderDetail.Price * createOrderDetail.Quantity;

                }
                createdOrder.OrderDetails = orderDetailList;
                createdOrder = await Update(createdOrder);

                var returna = _mapper.Map<OrderResponse>(createdOrder);
                returna.ConvertToOrderDetailResponse(orderDetailList, _mapper);
                return returna;
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

        public async Task<List<OrderResponse>> GetAllAsync(Guid accountId, string? search, string? sortBy, bool isDescending, OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize)
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

        public async Task<Order> Update(Order order)
        {
            try
            {
                var rover = await _orderRepository.UpdateAsync(order);
                return rover;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể update quantity, price", ex);
            }
        }

        public async Task<Order> UpdateStatus(Guid id, Order order)
        {
            try
            {
                var rover = await _orderRepository.UpdateStatusAsync(id,order);
                var ingredientProduct = new IngredientProduct();
                if (rover.OrderStatus == OrderStatus.CANCELED || rover.OrderStatus == OrderStatus.FAILED)
                {
                    var query = await _orderDetailService.GetAllOrderDetailsAsync(id,null,null,false,1,10);
                    var orderDetails = query.Select(q => new OrderDetail
                    {
                        Id = q.Id,
                        IngredientProductId = q.IngredientProductId,
                        Quantity = q.Quantity,
         
                    }).ToList();
                    //foreach (OrderDetail orderDetail in orderDetails)
                    //{
                    //    var chosenIngredientProduct = await _ingredientProductService.GetIngredientProductbyId(orderDetail.IngredientProductId);
                    //    chosenIngredientProduct.Quantity += orderDetail.Quantity;
                    //    await _ingredientProductService.UpdateIngredientProduct(chosenIngredientProduct);
                    //}
                }
                return rover;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể update status", ex);
            }
        }
    }


}
