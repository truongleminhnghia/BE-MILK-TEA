using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class IngredientRequest
    {
        [Required(ErrorMessage = "Nhà cung cấp là bắt buộc")]
        public string Supplier { get; set; }

        [Required(ErrorMessage = "Tên nguyên liệu là bắt buộc")]
        public string IngredientName { get; set; }
        public string Description { get; set; }
        public string FoodSafetyCertification { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được bỏ trống")]
        public DateTime ExpiredDate { get; set; }

        [Required(ErrorMessage = "Trạng thái nguyên liệu không được bỏ trống")]
        public IngredientStatus IngredientStatus { get; set; }

        [Required(ErrorMessage = "Số lượng trong mỗi túi không được bỏ trống")]
        [Range(0.1, float.MaxValue, ErrorMessage = "Số lượng trong mỗi túi phải lớn hơn 0")]
        public float WeightPerBag { get; set; }
        public int QuantityPerCarton { get; set; }

        [Required(ErrorMessage = "Loại nguyên liệu không được bỏ trống")]
        public IngredientType IngredientType { get; set; }

        [Required(ErrorMessage = "Đơn vị không được bỏ trống")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Giá không được bỏ trống")]
        public double PriceOrigin { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm không được bỏ trống")]
        public Guid CategoryId { get; set; }
        public bool IsSale { get; set; }
        public List<ImageRequest>? ImageRequest { get; set; }

        public List<IngredientQuantityRequest>? IngredientQuantities { get; set; }  
    }
}
