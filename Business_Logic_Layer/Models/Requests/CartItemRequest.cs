using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    class CartItemRequest
    {
        public int Quantity { get; set; }
        public string IngredientProductId { get; set; }

    }
}
