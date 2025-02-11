using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("order_detail")]
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("price")]
        [Required]
        public double Price { get; set; }

        [Column("ingredient_id")]
        [Required]
        public Guid IngredientId { get; set; }

        public Ingredient? Ingredient { get; set; }
    }
}