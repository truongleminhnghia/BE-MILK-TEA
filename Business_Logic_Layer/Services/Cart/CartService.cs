using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace Business_Logic_Layer.Services.Cart
{


    public class CartService
    {
        private readonly IAccountService _accountService;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemService _cartItemService;
        private readonly IMapper _mapper;
        private object accountService;

        public CartService(IAccountService accountService, ICartRepository cartRepository, ICartItemService cartItemService, IMapper mapper)
        {
            _accountService = accountService;
            _cartRepository = cartRepository;
            _cartItemService = cartItemService;
            this.accountService = accountService;
            _mapper = mapper;
        }

        public async Task<bool> CreateCart(CartRequest request)
        {
            try
            {
                // kiem tra accout ton tai chua 
                var accountResponse = await _accountService.GetById(request.AccountId);
                if (accountResponse != null)
                {
                    var cart = new Data_Access_Layer.Entities.Cart
                    {
                        AccountId = accountResponse.Id,
                        Account = _mapper.Map<Account>(accountResponse),
                        CreateAt = DateTime.UtcNow
                    };
                    await _cartRepository.Create(cart);

                    if (cart != null)
                    {
                        CartItem cartItem = new CartItem();
                        cartItem.CartId = cart.Id;
                        cartItem.Cart = cart;
                        cartItem.IngredientProductId = Guid.Parse(request.IngredientProductId);
                        cartItem.Quantity = request.Quantity;
                        await _cartItemService.Create(cartItem);
                    }
                    return true;
                }
                return false;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}