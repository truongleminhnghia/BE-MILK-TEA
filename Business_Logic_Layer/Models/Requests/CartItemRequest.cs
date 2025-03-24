using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class CartItemRequest
    {
        //public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int Quantity { get; set; }
        public Guid IngredientId { get; set; }
        public ProductType ProductType { get; set; }
        public bool IsCart { get; set; }
    }
}