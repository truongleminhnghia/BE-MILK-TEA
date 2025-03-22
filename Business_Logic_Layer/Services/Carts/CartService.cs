using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Org.BouncyCastle.Asn1.Misc;

namespace Business_Logic_Layer.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IAccountRepository _accountRepository;
        public IMapper _mapper;
        public CartService(ICartRepository cartRepository, IMapper mapper, IAccountRepository accountRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<CartResponse> CreateCart(Guid accountId)
        {
            var accountExisting = await _accountRepository.GetById(accountId);
            if (accountExisting == null)
            {
                throw new Exception("Tài khoảng không tồn tại");
            }
            Cart newCart = await CreateNewCart(accountExisting);
            return _mapper.Map<CartResponse>(newCart);
        }

        public async Task<CartResponse> GetByAccount(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                throw new ArgumentException("Account ID không hợp lệ", nameof(accountId));
            }

            var accountExisting = await _accountRepository.GetById(accountId);
            if (accountExisting == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại");
            }

            var cartExisting = await _cartRepository.FindByAccount(accountId);
            if (cartExisting == null)
            {
                cartExisting = await CreateNewCart(accountExisting);
            }

            CartResponse cartResponse = _mapper.Map<CartResponse>(cartExisting);
            if (cartExisting.CartItems == null)
            {
                cartResponse.CartItems = null;
                cartResponse.TotalCount = 0;
            }
            cartResponse.TotalCount = cartExisting.CartItems.Count;
            return cartResponse;
        }

        public async Task<CartResponse> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Cart ID không hợp lệ", nameof(id));
            }
            var cartExisting = await _cartRepository.FindById(id);
            if (cartExisting == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại");
            }
            return null;
        }

        private async Task<Cart> CreateNewCart(Account account)
        {
            Cart cart = new Cart();
            cart.AccountId = account.Id;
            cart.Account = account;
            cart.CreateAt = DateTime.Now;
            cart.UpdateAt = DateTime.Now;
            cart.CartItems = null;
            var newCart = await _cartRepository.Save(cart);
            if (newCart == null)
            {
                throw new Exception("Tạo cart không thành công");
            }
            return cart;
        }
    }
}