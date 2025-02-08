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
        public Guid Id { get; set; } = Guid.NewGuid();
        [Column("account_id")]
        public Guid AccountId {get; set; }
        public Account Account {get; set;}

        public virtual ICollection<Cart_Item> Cart_Items { get; set; } = new List<Cart_Item>();
    }
}