using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class UpdateIngredientRequest
    {
        public string? Supplier { get; set; }
        public string? IngredientName { get; set; }
        public string? Description { get; set; }
        public string? FoodSafetyCertification { get; set; }
        public IngredientStatus IngredientStatus { get; set; }
        public float? WeightPerBag { get; set; }
        public int? QuantityPerCarton { get; set; }
        public double? PriceOrigin { get; set; }
        public bool? IsSale { get; set; }
        public List<ImageRequest>? ImageRequest { get; set; }
        public List<IngredientQuantityRequest>? IngredientQuantities { get; set; }
    }
}