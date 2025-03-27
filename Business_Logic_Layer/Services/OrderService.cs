using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.Carts;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Business_Logic_Layer.Services.PromotionDetailService;
using Business_Logic_Layer.Services.PromotionService;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using MailKit.Search;
using Microsoft.Identity.Client;



namespace Business_Logic_Layer.Services
{
    public interface IOrderService
    {
        public Task<OrderResponse> CreateAsync(OrderRequest order);
        public Task<List<OrderResponse>> GetAllAsync(Guid accountId, string? search, string? sortBy, bool isDescending, OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize);
        public Task<OrderResponse> GetByIdAsync(Guid orderId);
        public Task<OrderResponse> GetByCodeAsync(String orderCode);
        public Task<Order> Update(Order orderId);
        public Task<Order> UpdateStatus(Guid id, Order order);
        //public Task<bool> DeleteByIdAsync(Guid orderId);

    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IIngredientService _ingredientService;
        private readonly IPromotionService _promotionService;
        private readonly IPromotionDetailService _promotionDetailService;
        private readonly IIngredientQuantityService _ingredientQuantityService;
        private readonly ICartItemService _cartItemService;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IOrderDetailService orderDetailService, Source source, IIngredientQuantityService ingredientQuantityService, ICartItemService cartItemService , IIngredientService ingredientService, IPromotionDetailService promotionDetailService, IPromotionService promotionService )
        {
            _promotionDetailService = promotionDetailService;
            _promotionService = promotionService;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderDetailService = orderDetailService;
            _source = source;
            _ingredientService = ingredientService;
            _ingredientQuantityService = ingredientQuantityService;
            _cartItemService = cartItemService;
            //_ingredientProductService = ingredientProductService;
        }
        public async Task<OrderResponse> CreateAsync(OrderRequest orderRequest)
        {
            try
            {
                var order = _mapper.Map<Order>(orderRequest);
                order.OrderCode = "OD" + _source.GenerateRandom8Digits();
                order.OrderDate = DateTime.Now;
                order.OrderStatus= OrderStatus.PENDING_CONFIRMATION;

                double totalDiscount = 0;
                double finalPrice = 0;

                var createdOrder = await _orderRepository.CreateAsync(order);
                List<OrderDetail> orderDetailList = new List<OrderDetail>();

                foreach (OrderDetailRequest orderDetail in orderRequest.orderDetailList)
                {
                    //xử lý quantity
                    var orderDetails = new OrderDetail();
                    var cartItem = await _cartItemService.GetById(orderDetail.CartItemId);
                    var ingredientProduct = await _ingredientService.GetById(cartItem.IngredientId);

                    if (ingredientProduct == null)
                    {
                        throw new Exception($"Không tìm thấy ingredientProduct với ID {cartItem.IngredientId}");
                    }

                    var ingredientQuantityProduct = await _ingredientQuantityService.GetByIdAndProductType(ingredientProduct.Id, cartItem.ProductType);

                    if (ingredientQuantityProduct == null)
                    {
                        throw new Exception($"Không tìm thấy nguyên liệu với ID {ingredientProduct.Id}");
                    }

                    if (ingredientQuantityProduct.Quantity < cartItem.Quantity)
                    {
                        throw new Exception($"Số lượng đặt hàng ({cartItem.Quantity}) vượt quá số lượng sẵn có ({ingredientQuantityProduct.Quantity})");
                    }
                    //Trừ số lượng đang có trong database = số lượng đặt
                    ingredientQuantityProduct.Quantity -= cartItem.Quantity;

                    //Gán ingredientQuantityProduct thành IngredientQuantityRequest
                    var ingredientQuantityRequest = _mapper.Map<IngredientQuantityRequest>(ingredientQuantityProduct);
                    await _ingredientQuantityService.UpdateAsync(ingredientQuantityProduct.Id, ingredientQuantityRequest);

                    //tạo orderdetail
                    
                    orderDetails.OrderId = createdOrder.Id;
                    orderDetails.CartItemId = orderDetail.CartItemId;
                    orderDetails.Quantity = cartItem.Quantity;
                    orderDetails.Price = ingredientProduct.PriceOrigin;

                    var createOrderDetail = await _orderDetailService.CreateAsync(orderDetails);
                    orderDetailList.Add(createOrderDetail);
                    createdOrder.Quantity += createOrderDetail.Quantity;
                    createdOrder.TotalPrice += ingredientProduct.PriceOrigin * createOrderDetail.Quantity;

                }

                finalPrice = createdOrder.TotalPrice;
                if (!string.IsNullOrEmpty(orderRequest.PromotionCode))
                {
                    var promotion = await _promotionService.GetByCodeAsync(orderRequest.PromotionCode);
                    if (promotion == null || !promotion.IsActive || promotion.EndDate < DateTime.Now)
                    {
                        throw new Exception("Mã khuyến mãi không hợp lệ hoặc đã hết hạn.");
                    }

                    if (promotion.PromotionType != PromotionType.PROMOTION_ORDER)
                    {
                        throw new Exception("Mã khuyến mãi không áp dụng cho đơn hàng.");
                    }

                    var promotionDetail = _promotionDetailService.GetbyPromotionId(promotion.Id);
                    if (promotionDetail == null)
                    {
                        throw new Exception("Không tìm thấy thông tin chi tiết khuyến mãi.");
                    }

                    var promotionDetailValue = await promotionDetail;
                    // Kiểm tra tổng giá trị đơn hàng có đạt mức tối thiểu không
                    if (createdOrder.TotalPrice <= promotionDetailValue.MiniValue)
                    {
                        throw new Exception($"Đơn hàng chưa đạt giá trị tối thiểu để áp dụng khuyến mãi ({promotionDetailValue.MiniValue} VND).");
                    }
   
                    // Tính toán số tiền giảm giá
                    double discountValue = promotionDetailValue.DiscountValue;
                    double maxDiscount = promotionDetailValue.MaxValue;

                    if (maxDiscount >= createdOrder.TotalPrice)
                    {
                        totalDiscount = createdOrder.TotalPrice * discountValue / 100;
                    }
                    else if (maxDiscount < createdOrder.TotalPrice)
                    {
                        totalDiscount = maxDiscount * discountValue / 100;
                    }

                    // Cập nhật tổng giá trị đơn hàng sau giảm giá
                    finalPrice = createdOrder.TotalPrice - totalDiscount;

                    // Lưu thông tin khuyến mãi vào bảng OrderPromotion
                    var orderPromotion = new OrderPromotion
                    {
                        OrderId = createdOrder.Id,
                        PromotionId = promotion.Id
                    };
                    await _promotionService.CreateOrderPromotionAsync(orderPromotion);
                    createdOrder.PriceAffterPromotion = finalPrice;
                }
                else
                {
                    createdOrder.PriceAffterPromotion = 0;
                }
                
                
                createdOrder.OrderDetails = orderDetailList;
                createdOrder = await Update(createdOrder);

                var returna = _mapper.Map<OrderResponse>(createdOrder);
                returna.TotalPrice = createdOrder.TotalPrice; // Giá gốc
                returna.PriceAfterPromotion = createdOrder.PriceAffterPromotion;
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

        public async Task<OrderResponse> GetByCodeAsync(string orderCode)
        {
            try
            {
                var order = await _orderRepository.GetByCodeAsync(orderCode);
                return order == null ? null : _mapper.Map<OrderResponse>(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin order bằng id", ex);
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
                        CartItemId = q.CartItemId,
                        Quantity = q.Quantity,
         
                    }).ToList();
                    foreach (OrderDetail orderDetail in orderDetails)
                    {
                        var cartItemId = orderDetail.CartItemId;
                        var cartItem = await _cartItemService.GetById(cartItemId);
                        var ingredient = await _ingredientService.GetById(cartItem.IngredientId);
                        if (cartItem == null)
                        {
                            throw new Exception($"Không tìm thấy cartItem với ID {cartItemId}");
                        }

                        var ingredientQuantityProduct = await _ingredientQuantityService.GetByIdAndProductType(ingredient.Id, cartItem.ProductType);

                        if (ingredientQuantityProduct == null)
                        {
                            throw new Exception($"Không tìm thấy nguyên liệu với ID {ingredient.Id}");
                        }
                        ingredientQuantityProduct.Quantity += orderDetail.Quantity;
                        var a = _mapper.Map<IngredientQuantityRequest>(ingredientQuantityProduct);
                        await _ingredientQuantityService.UpdateAsync(ingredientQuantityProduct.Id, a);
                    }
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
