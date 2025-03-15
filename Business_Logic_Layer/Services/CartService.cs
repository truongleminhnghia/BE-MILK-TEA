using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Services
{

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public CartService(ICartRepository cartRepository, IMapper mapper, IAccountRepository accountRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _cartItemRepository = cartItemRepository;
        }

        public Task<CartResponse> CreateAsync(CartRequest cart)
        {
            throw new NotImplementedException();
        }

        public async Task<CartResponse?> GetByAccountAsync(Guid id)
        {
            CartResponse cartResponse = null;
            var account = await _accountRepository.GetById(id);
            if (account == null)
            {
                throw new Exception("Tài khoảng không tồn tại");
            }
            var cartEixst = await _cartRepository.GetByAccountAsync(account.Id);
            if (cartEixst == null)
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
                cartResponse.CarItemResponse = null;
                cartResponse.TotalCartItem = 0;
            }
            cartResponse.Id = cartEixst.Id;
            cartResponse.AccountResponse = _mapper.Map<AccountResponse>(cartEixst.Account);
            cartResponse.CarItemResponse = _mapper.Map<CartItemResponse>(cartEixst.CartItems);
            cartResponse.TotalCartItem = 0;
        }

        public Task<bool?> UpdateAsync(Guid id, CartRequest request)
        {
            throw new NotImplementedException();
        }

        private int SumCartItem(List<CartItem> cartItems)
        {
            try
            {
                int count = 0;
                // List
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return -1;
            }
        }

        Task<CartResponse?> ICartService.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
