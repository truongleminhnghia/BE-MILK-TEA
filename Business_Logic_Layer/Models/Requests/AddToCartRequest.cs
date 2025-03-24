using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class AddToCartRequest
    {
        public Guid AccountId { get; set; }
        public Guid IngredientId { get; set; }

        public int Quantity { get; set; }
 
    }
}
