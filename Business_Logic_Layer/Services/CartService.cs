using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
//using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace Business_Logic_Layer.Services
{
   
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, ApplicationDbContext dbContext, IAccountRepository accountRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _context = dbContext;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task AddToCartAsync(Guid accountId, Guid ingredientProductId, int quantity)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity; // Cập nhật số lượng
                existingCartItem.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                try
                {
                    cart.CartItems.Add(new CartItem
                    {
                        CartId = cart.Id,
                        IngredientProductId = ingredientProductId,
                        Quantity = quantity,
                        UpdateAt=DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public async Task<CartResponse> GetByIdAsync(Guid accountId)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            // Lấy giỏ hàng hoặc tạo mới nếu chưa có
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            // Chuyển đổi danh sách CartItem thành CartItemResponse
            var cartItemsResponse = cart.CartItems?.Select(ci => new CartItemResponse
            {
                CartItemId = ci.Id,
                IngredientProductId = ci.IngredientProductId,
                Quantity = ci.Quantity
            }).ToList() ?? new List<CartItemResponse>();

            // Tạo đối tượng phản hồi
            return new CartResponse
            {
                CartId = cart.Id,
                AccountResponse = _mapper.Map<AccountResponse>(account),
                CartItems = cartItemsResponse,
                TotalCartItem = cartItemsResponse.Sum(ci => ci.Quantity) // Tổng số lượng sản phẩm trong giỏ
            };
        }


        public async Task<bool> RemoveItemAsync(Guid accountId, Guid ingredientProductId)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            if (!cart.CartItems.Any())
                return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (cartItem != null)
            {
                await _cartItemRepository.RemoveCartItemByIdAsync(cartItem.Id);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<bool> UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);
            if (cart.CartItems == null || !cart.CartItems.Any())
                return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);
            if (cartItem == null)
                return false;

            cartItem.Quantity = quantity;
            await _cartRepository.UpdateCartItemQuantityAsync(accountId, ingredientProductId, quantity);
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid accountId)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);
            if (cart.CartItems == null || !cart.CartItems.Any())
                return false;

            await _cartRepository.ClearCartAsync(accountId);
            return true;
        }

        private async Task<int> SumCartItem(Guid id, List<CartItem> cartItems)
        {
            try
            {
                int count = 0;
                IEnumerable<CartItem> cartItemsFromDb = await _cartItemRepository.GetByCart(id);
                if (cartItemsFromDb == null)
                {
                    count = 0;
                }
                count = cartItemsFromDb.Count();
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return -1;
            }
        }

        public async Task<CartResponse> CreateAsync(CartRequest request)
        {
            try
            {
                // Gọi hàm repo để tạo giỏ hàng
                var createdCart = await _cartRepository.GetOrCreateCartAsync(request.AccountId);
                var cartResponse = _mapper.Map<CartResponse>(createdCart);

                return cartResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo giỏ hàng", ex);
            }
        }
    }
}
