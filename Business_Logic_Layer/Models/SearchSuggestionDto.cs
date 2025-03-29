using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models
{
    public  class SearchSuggestionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }  // "Recipe", "Ingredient"
        public string Title { get; set; }
    }
}
