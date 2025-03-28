using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer.Services.Carts
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IIngredientQuantityRepository _ingedientQuantity;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IAccountRepository _accountRepository;

        public CartItemService(ICartItemRepository cartItemRepository, ICartRepository cartRepository, IMapper mapper,
                                IAccountRepository accountRepository, IIngredientQuantityRepository ingedientQuantity,
                                IIngredientRepository ingredientRepository
        )
        {
            _cartItemRepository = cartItemRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
            _ingedientQuantity = ingedientQuantity;
            _ingredientRepository = ingredientRepository;
        }

        private async Task<bool> CheckQuantity(int Quantity, ProductType productType, Guid id)
        {
            try
            {
                if (id == null || Quantity == 0 || productType == null)
                {
                    throw new Exception("số lượng không được nhỏ hơn 0 hoặc loại sản phẩm ko được trống");
                }
                var result = await _ingedientQuantity.GetByIdAndProductType(id, productType);
                if (result == null)
                {
                    throw new Exception("Không tồn tại số lượng dựa trên loại yêu cầu");
                }
                else if (result != null && Quantity > result.Quantity)
                {
                    throw new Exception($"Số lượng của loại {productType.ToString()} không còn đủ");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: ", ex.Message);
                return false;
            }
        }

        private async Task<Cart> CreateNewCart(Account account)
        {
            Cart cart = new Cart();
            cart.AccountId = account.Id;
            cart.Account = account;
            cart.CreateAt = DateTime.Now;
            cart.UpdateAt = DateTime.Now;
            var newCart = await _cartRepository.Save(cart);
            if (newCart == null)
            {
                throw new Exception("Tạo cart không thành công");
            }
            return cart;
        }

        public double GetPrice(Ingredient ingredient)
        {
            double price = 0.0;
            if (ingredient.PricePromotion > 0.0)
            {
                price += ingredient.PricePromotion;
            }
            else
            {
                price += ingredient.PriceOrigin;
            }
            return price;
        }

        public async Task<CartItemResponse> CreateCartItem(CartItemRequest cartItemRequest)
        {
            var account = await _accountRepository.GetById(cartItemRequest.AccountId);
            if (account == null)
            {
                throw new Exception("Cart không tồn tại");
            }
            var ingre = await _ingredientRepository.GetById(cartItemRequest.IngredientId);  
            if (ingre == null)
            {
                throw new Exception("Nguyên liệu không tồn tại");
            }
            bool result = await CheckQuantity(cartItemRequest.Quantity, cartItemRequest.ProductType, cartItemRequest.IngredientId);
            if (!result)
            {
                throw new Exception("Số lượng không phù hợp");
            }

            var cartExisting = await _cartRepository.FindByAccount(cartItemRequest.AccountId);
            if (cartExisting == null)
            {
                cartExisting = await CreateNewCart(account);
            }


            // **Tìm CartItem có cùng IngredientId trong giỏ hàng**
            var existingCartItem = cartExisting.CartItems
     .FirstOrDefault(ci => ci.IngredientId == cartItemRequest.IngredientId
                        && ci.ProductType == cartItemRequest.ProductType);

            // **Nếu IsCart == true và đã có nguyên liệu trong giỏ hàng => Báo lỗi**
            if (cartItemRequest.IsCart && existingCartItem != null)
            {
                throw new Exception("Đã có item trong giỏ hàng rồi");
            }

            // **Tạo mới CartItem, không cộng dồn số lượng**
            CartItem item = _mapper.Map<CartItem>(cartItemRequest);
            item.CartId = cartExisting.Id;
            item.CreateAt = DateTime.Now;
            item.UpdateAt = DateTime.Now;

            double OriginPrice = GetPrice(ingre);
            double OriginTotalPrice = OriginPrice * item.Quantity;

            if (cartItemRequest.IsCart)
            {
                item.Price = 0;
                item.TotalPrice = 0; // Khi là giỏ hàng, lưu giá = 0
            }
            else
            {
                item.Price = OriginPrice;
                item.TotalPrice = OriginTotalPrice;
            }

            CartItem cartItem = await _cartItemRepository.SaveCartItem(item);
            cartExisting.CartItems.Add(cartItem);

            // **Tạo response trả về**
            CartItemResponse cartItemResponse = _mapper.Map<CartItemResponse>(cartItem);
            return cartItemResponse;
        }

        

        public async Task<bool> Delete(Guid id)
        {
            if (id == null)
            {
                throw new Exception("Id không được bỏ trống");
            }
            var cartItem = await _cartItemRepository.GetById(id);
            if (cartItem == null)
            {
                throw new Exception("Cart Item không tồn tại");
            }
            return await _cartItemRepository.DeleteCartItem(cartItem);
        }

        public async Task<IEnumerable<CartItemResponse>> GetByCart(Guid cartId)
        {
            if (cartId == null)
            {
                throw new Exception("card id khong được rỗng");
            }
            var cartItems = await _cartItemRepository.GetByCart(cartId);
            if (cartItems == null)
            {
                throw new Exception("Cart rỗng");
            }
            return _mapper.Map<IEnumerable<CartItemResponse>>(cartItems);
        }

        public async Task<CartItemResponse> GetById(Guid id)
        {
            if (id == null)
            {
                throw new Exception("Id không được bỏ trống");
            }
            var cartItem = await _cartItemRepository.GetById(id);
            if (cartItem == null)
            {
                throw new Exception("Cart Item không tồn tại");
            }
            CartItemResponse cartItemResponse = _mapper.Map<CartItemResponse>(cartItem);
            cartItemResponse.Price = GetPrice(cartItem.Ingredient);
            cartItemResponse.TotalPrice = cartItem.Quantity * cartItemResponse.Price;
            return cartItemResponse;
        }

        public async Task<bool> UpdateCartItem(Guid id, UpdateCartItemRequest request)
        {
            var cartItem = await _cartItemRepository.GetById(id);

            if (cartItem == null)
            {
                return false; // Không tìm thấy cart item
            }
            // Cập nhật quantity và product type
            cartItem.Quantity = request.Quantity;
            cartItem.TotalPrice = cartItem.Price * request.Quantity;
            //cartItem.ProductType = request.ProductType;

            _cartItemRepository.UpdateCartItem(cartItem);
            return true;
        }
        public async Task<bool> UpdateCartItemStatus(Guid cartItemId, bool isCart)
        {
            return await _cartItemRepository.UpdateCartItemStatus(cartItemId, isCart);
        }
    }
}