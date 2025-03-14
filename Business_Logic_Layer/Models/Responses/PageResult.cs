using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class PageResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageCurrent { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}