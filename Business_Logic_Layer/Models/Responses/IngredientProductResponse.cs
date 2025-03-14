using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class IngredientProductResponse
    {
        public Guid? Id { get; set; }

        public Guid IngredientId { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public ProductType ProductType { get; set; }
    }
}
