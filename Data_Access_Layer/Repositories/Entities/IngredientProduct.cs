using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("ingredient_product")]
    public class IngredientProduct
    {
        [Key]
        [Column("ingredient_product_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Column("total_price")]
        [Required]
        public double TotalPrice { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        public Ingredient? Ingredient { get; set; }
    }
}