using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("category")]
    public class Category : BaseEntity
    {
        [Key]
        [Column("category_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("category_name", TypeName = "nvarchar(300)")]
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Column("category_status")]
        [Required]
        [EnumDataType(typeof(CategoryStatus))]
        public CategoryStatus CategoryStatus { get; set; }

        [Column("category_type")]
        [Required]
        [EnumDataType(typeof(CategoryType))]
        public CategoryType CategoryType { get; set; }

        //relationship
        // 1-N product
        // 1-N Recipe
        public ICollection<Ingredient>? Ingredients { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }
    }
}