using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class CategoryRequest
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public CategoryStatus CategoryStatus { get; set; }
        public CategoryType CategoryType { get; set; }

    }
}