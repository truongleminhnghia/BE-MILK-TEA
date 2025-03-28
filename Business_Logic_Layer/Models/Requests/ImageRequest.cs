using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class ImageRequest
    {
        public Guid? Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
