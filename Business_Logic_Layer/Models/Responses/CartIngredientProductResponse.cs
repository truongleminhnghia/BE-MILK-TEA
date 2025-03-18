using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class CartIngredientProductResponse
    {
        public Guid? Id { get; set; }
        public Guid IngredientId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public ProductType ProductType { get; set; }

        // Thay thế Ingredient bằng DTO mới
        public CartIngredientResponse Ingredient { get; set; }
    }
}
