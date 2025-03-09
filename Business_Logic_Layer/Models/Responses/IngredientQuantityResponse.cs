using Data_Access_Layer.Enum;
using System.ComponentModel.DataAnnotations;

namespace Business_Logic_Layer.Models.Responses
{
    public class IngredientQuantityResponse
    {
        public Guid Id { get; set; }

        public Guid IngredientId { get; set; }

        public int Quantity { get; set; }

        public ProductType ProductType { get; set; }
    }
}