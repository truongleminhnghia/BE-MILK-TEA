using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("ingredient_review")]
    public class IngredientReview
    {
        [Key]
        [Column("feedback_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Column("account_id")]
        [Required]
        public Guid AccountId { get; set; }

        [Column("comment", TypeName = "nvarchar(500)")]
        public string? Comment { get; set; }

        [Column("rate")]
        public double Rate { get; set; }

        //relationship
        // N-1 Account
        // N-1 Product
        public Ingredient? Ingredient { get; set; }

        public Account? Account { get; set; }

    }
}