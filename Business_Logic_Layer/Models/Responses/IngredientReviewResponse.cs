using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class IngredientReviewResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public Guid AccountId { get; set; }
        public string? Comment { get; set; }
        public double Rate { get; set; }
        public string? IngredientName { get; set; }
        public string? AccountName { get; set; }
    }
}
