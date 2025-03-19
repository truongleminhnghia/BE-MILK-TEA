using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public AccountResponse? Account { get; set; }
        public List<CartItemResponse> CartItems { get; set; }
        public int TotalCount { get; set; }
    }
}