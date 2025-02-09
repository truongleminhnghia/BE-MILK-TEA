using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class IngredientRecipe
    {
        public Guid IngredientId { get; set; }
        public Guid RecipeId { get; set; }

        //     setup relationship
        public virtual Ingredient Ingredient { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
