using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    [Table("cart")]
    public class Cart
    {
        [Key]
        [Column("cart_id")]
        public Guid Id { get; set; }
        [Column("account_id")]
        public Guid AccountId {get; set; }
        public Account Account {get; set;}

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public Cart()
        {
            AccountId = Guid.NewGuid();
        }
    }
}