using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class PromotionDetailResponse
    {
        [Required]
        public Guid Id { get; set; }

        public string? PromotionName { get; set; }
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "DiscountValue phải nằm trong khoảng 0 - 100")]
        public float? DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinValue phải là số dương")]
        public double? MinValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxValue phải là số dương")]
        public double? MaxValue { get; set; }

        public Guid? PromotionId { get; set; }
    }
}
