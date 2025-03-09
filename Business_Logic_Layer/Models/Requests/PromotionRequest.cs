using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class PromotionRequest
    {
        public string PromotionCode { get; set; }
        public PromotionType PromotionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public List<Guid> IngredientIds { get; set; } = new();
        public PromotionDetailRequest PromotionDetails { get; set; }
    }

    public class PromotionDetailRequest
    {
        public Guid? Id { get; set; }

        public string? PromotionName { get; set; }

        public string? Description { get; set; }

        public float DiscountValue { get; set; }

        public double MiniValue { get; set; }

        public double MaxValue { get; set; }

        public Guid? PromotionId { get; set; }
    }

}
