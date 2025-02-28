using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientRequest
    {
        public string Supplier { get; set; }
        public string IngredientName { get; set; }
        public string Description { get; set; }
        public string FoodSafetyCertification { get; set; }
        public DateTime ExpiredDate { get; set; }
        public IngredientStatus IngredientStatus { get; set; }
        public float WeightPerBag { get; set; }
        public int QuantityPerCarton { get; set; }
        public IngredientType IngredientType { get; set; }
        public string Unit { get; set; }
        public double PriceOrigin { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsSale { get; set; }
        public List<ImageRequest>? ImageRequest { get; set; }
    }
}
