using Data_Access_Layer.Enum;
using System.ComponentModel.DataAnnotations;


namespace Business_Logic_Layer.Models.Requests
{
    public class CategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        [Required(ErrorMessage = "CategoryStatus is required")]
        public CategoryStatus CategoryStatus { get; set; }
        [Required(ErrorMessage = "CategoryType is required")]
        public CategoryType CategoryType { get; set; }

    }
}
