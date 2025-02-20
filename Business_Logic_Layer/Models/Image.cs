using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        public Guid IngredientId { get; set; }
    }
}
