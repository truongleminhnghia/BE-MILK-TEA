using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
public class CartItemResponse
{
        public Guid CartItemId { get; set; }
        public Guid IngredientProductId { get; set; }
    public int Quantity { get; set; }
    }
}