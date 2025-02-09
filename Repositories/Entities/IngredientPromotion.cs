using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class IngredientPromotion
    {
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Column("promotion_id")]
        public Guid PromotionId { get; set; }

        //      setup relationship
        public virtual Ingredient? Ingredient { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
