using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; } // ko nhận id
        // public string IngredientCode { get; set; } // ko nhận request
        [Required(ErrorMessage ="aaaaaaa")]
        public string Supplier { get; set; }
        public string IngredientName { get; set; }
        public string Description { get; set; }
        public string FoodSafetyCertification { get; set; }
        public DateTime ExpiredDate { get; set; } // 
        public IngredientStatus IngredientStatus { get; set; }
        public float WeightPerBag { get; set; }
        public int QuantityPerCarton { get; set; }
        public int Unit { get; set; }
        public double PriceOrigin { get; set; }
        public double PricePromotion { get; set; }
        public Guid CategoryId { get; set; }
        public int Quantity { get; set; }
        public bool IsSale { get; set; }
        public float Rate { get; set; } // ko nhận rate
    }
}
