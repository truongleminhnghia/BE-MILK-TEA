using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientQuantityRequest
    {
        [Required(ErrorMessage = "Chưa có nguyên liệu")]
        public Guid IngredientId { get; set; }

        [Range(1, int.MinValue, ErrorMessage = "Quantity must be greater than 0")]
        [Required]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Chưa có loại hàng")]
        public ProductType ProductType { get; set; }
    }
}