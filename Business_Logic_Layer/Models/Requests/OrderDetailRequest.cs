using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderDetailRequest
    {
        public Guid OrderId { get; set; }
        public Guid IngredientProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
