using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public string IngredientCode { get; set; }
        public string Supplier { get; set; }
        public string IngredientName { get; set; }
        public string Description { get; set; }
        public string FoodSafetyCertification { get; set; }
        public DateTime ExpiredDate { get; set; }
        public IngredientStatus IngredientStatus { get; set; }
        public float WeightPerBag { get; set; }
        public int QuantityPerCarton { get; set; }
        public UnitOfIngredientEnum Unit { get; set; }
        public double PriceOrigin { get; set; }
        public double PricePromotion { get; set; }
        public CategoryResponse? Category { get; set; }
        public bool IsSale { get; set; }
        public float Rate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string IngredientType { get; set; }

        public List<ImageResponse?> Images { get; set; }

        public List<IngredientQuantityResponse>? IngredientQuantities { get; set; }

        public List<IngredientReviewResponse>? IngredientReviews { get; set; }
    }
}
