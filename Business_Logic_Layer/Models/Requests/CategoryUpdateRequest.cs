using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class CategoryUpdateRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        [Required(ErrorMessage = "CategoryStatus is required")]
        public CategoryStatus CategoryStatus { get; set; }
    }
}
