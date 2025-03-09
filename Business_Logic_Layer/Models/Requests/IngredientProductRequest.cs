using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientProductRequest
    {
        public Guid IngredientId { get; set; }
        public int Quantity { get; set; }

        public ProductType ProductType { get; set; }
    }
}
