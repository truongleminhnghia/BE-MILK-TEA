using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Guid IngredientProductId { get; set; }

        public string ingredientName { get; set; }
        public string imgURL { get; set; }
    }
}
