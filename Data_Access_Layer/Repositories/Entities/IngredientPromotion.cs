using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("ingredient_promotion")]
    public class IngredientPromotion
    {
        [Key]
        [Column("ingredient_promotion_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("ingredient_id")]
        public Guid IngredientId { get; set; }

        public Ingredient? Ingredient { get; set; }

        [Column("promotion_id")]
        [ForeignKey("promotion_id")]
        [Required]
        public Guid PromotionId { get; set; }

        public Promotion? Promotion { get; set; }
    }
}