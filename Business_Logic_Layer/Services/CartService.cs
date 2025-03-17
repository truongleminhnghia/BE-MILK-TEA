using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.DTO;
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
            CartResponse cartResponse = new CartResponse();
            var account = await _accountRepository.GetById(accountId);
            if (account == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }
            Cart? cartEixst = await _cartRepository.GetByAccountAsync(account.Id);
            if (cartEixst != null)
            {
                cartResponse.Id = cartEixst.Id;
                cartResponse.AccountResponse = _mapper.Map<AccountResponse>(cartEixst.Account);
                cartResponse.CarItemResponse = cartEixst.CartItems != null
                         ? _mapper.Map<CartItemResponse>(cartEixst.CartItems)
                         : null;

                cartResponse.TotalCartItem = await SumCartItem(
                        cartEixst.Id,
                        cartEixst.CartItems?.ToList() ?? new List<CartItem>() // Ensure no null issue
        );
            }
            else
            {
                Cart cart = new Cart();
                cart.AccountId = account.Id;
                var result = await _cartRepository.CreateAsync(cart);
                if (result == null)
                {
                    throw new Exception("Tạo cart không thành công");
                }
                cartResponse.Id = result.Id;
                cartResponse.AccountResponse = _mapper.Map<AccountResponse>(account);
                // cartResponse.CarItemResponse = null;
                cartResponse.TotalCartItem = 0;
            }

            return cartResponse;
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
        public async Task<CartResponse> CreateAsync(CartRequest cart)
        {
            try
            {
                var reqCart = _mapper.Map<Cart>(cart);
                // Gọi hàm repo để tạo giỏ hàng
                var createdCart = await _cartRepository.CreateAsync(reqCart);
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
