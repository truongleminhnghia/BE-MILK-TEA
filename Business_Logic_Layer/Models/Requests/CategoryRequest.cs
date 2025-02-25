using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class CategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public CategoryStatus CategoryStatus { get; set; }
        public CategoryType CategoryType { get; set; }

    }
}
