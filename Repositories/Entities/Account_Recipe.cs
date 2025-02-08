using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class Account_Recipe
    {
        [Column("account_id")]
        public Guid Account_Id { get; set; }

        [Column("recipe_id")]
        public Guid Recipe_Id { get; set; }

        //     setup relationship
        public virtual Account Account { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}