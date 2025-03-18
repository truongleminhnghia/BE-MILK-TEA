﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services
{
    public interface ICartService
    {
        Task<CartResponse?> GetByIdAsync(Guid accountId);
        Task AddToCartAsync(Guid accountId, Guid ingredientProductId, int quantity);
        Task<bool> RemoveItemAsync(Guid accountId, Guid ingredientProductId);
        Task<bool> UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity);
        Task<bool> ClearCartAsync(Guid accountId);
        public Task<CartResponse> CreateAsync(CartRequest request);
    }
}