using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class PromotionDetailRequest
    {
        public string PromotionName { get; set; }

        public string? Description { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "DiscountValue phải nằm trong khoảng 0 - 100")]
        public float DiscountValue { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "MiniValue phải là số dương")]
        public double MiniValue { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "MaxValue phải là số dương")]
        public double MaxValue { get; set; }


    }
}
