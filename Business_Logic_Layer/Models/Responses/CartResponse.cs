using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Models.Responses
{
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public Guid AccountId { get; set; }
        public List<CartItemResponse> CartItems { get; set; } = new();
    }
}
