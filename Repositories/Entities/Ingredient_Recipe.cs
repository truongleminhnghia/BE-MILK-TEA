using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Ingredient_Recipe
    {
        public Guid Ingredient_Id { get; set; }
        public Guid Recipe_Id { get; set; }

        //     setup relationship
        public virtual Ingredient Ingredient { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
