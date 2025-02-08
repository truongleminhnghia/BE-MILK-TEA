using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Ingredient_Promotion
    {
        [Column("ingredient_id")]
        public Guid Ingredient_Id { get; set; }

        [Column("promotion_id")]
        public Guid Promotion_Id { get; set; }
        //      setup relationship
        public virtual Ingredient? Ingredient { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
