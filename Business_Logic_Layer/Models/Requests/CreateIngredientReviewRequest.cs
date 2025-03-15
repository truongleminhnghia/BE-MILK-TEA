using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class CreateIngredientReviewRequest
    {
        [Required]
        public Guid IngredientId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Range(0, 5)]
        [Required(ErrorMessage = "Bạn chưa nhập số sao đánh giá.")]
        public double Rate { get; set; }
    }

    public class UpdateIngredientReviewRequest
    {
        [StringLength(500)]
        public string? Comment { get; set; }

        [Range(0, 5)]
        public double Rate { get; set; }
    }
}
