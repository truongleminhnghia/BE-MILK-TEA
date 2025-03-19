using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Models.Responses
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public AccountResponse? AccountResponse { get; set; }
        public List<CartItemResponse>? CartItemResponse { get; set; }
        public int TotalCartItem { get; set; }

    }
}
