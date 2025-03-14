using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("cart")]
    public class Cart : BaseEntity
    {
        [Key]
        [Column("cart_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Column("account_id")]
        [Required]
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public Account Account { get; set; }

        public ICollection<CartItem> CartItems { get; set; }


        // relationship
        // 1-1 Account
        // 1-N CartItem

    }
}