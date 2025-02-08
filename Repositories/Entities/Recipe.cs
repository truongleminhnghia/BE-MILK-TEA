using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Recipe
    {
        [Key]
        [Column ("recipe_id")]
        public Guid Recipe_Id { get; set; } = Guid.NewGuid();

        [Column ("recipe_title", TypeName = "nvarchar(300)")]
        public string Recipe_title { get; set; }

        [Column ("content", TypeName = "nvarchar")]
        [MaxLength (1000)]
        public string Content { get; set; }

        [Column("category_id")]
        public Guid Category_Id { get; set; }

        //      setup relationship
        public virtual Category Category { get; set; }
        public virtual ICollection<Ingredient_Recipe> Ingredients_Recipes { get; set; }
        public virtual ICollection<Account_Recipe> Accounts_Recipes { get; set; }


    }
}
