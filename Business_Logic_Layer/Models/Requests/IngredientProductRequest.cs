using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientProductRequest
    {
        [Required(ErrorMessage = "Chưa có nguyên liệu.")]
        public Guid IngredientId { get; set; }

        [Required(ErrorMessage = "Chưa có số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        [Required(ErrorMessage = "Chưa có loại hàng.")]
        public ProductType ProductType { get; set; }
    }
}
