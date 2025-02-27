using Data_Access_Layer.Enum;
using System.Text.Json.Serialization;


namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientRequest
    {
        [JsonIgnore]
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
        public string Unit { get; set; }
        public double PriceOrigin { get; set; }
        public double PricePromotion { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsSale { get; set; }

        [JsonIgnore]
        public float Rate { get; set; }
    }
}
