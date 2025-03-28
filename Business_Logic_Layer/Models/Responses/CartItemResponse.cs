using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid IngredientId { get; set; }
        public IngredientResponse? Ingredient { get; set; }
        public int Quantity { get; set; }
        public ProductType ProductType { get; set; }
        public double TotalPrice { get; set; }
        public bool IsCart { get; set; }
        public double Price { get; set; }
        public bool isCart { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}