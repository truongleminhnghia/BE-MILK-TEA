using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("customer")]
    public class Customer : BaseEntity
    {
        [Key]
        [Column("customer_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("account_id")]
        [Required]
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        [Column("tax_code", TypeName = "varchar(200)")]
        public string TaxCode { get; set; } = string.Empty;

        [Column("address", TypeName = "nvarchar(500)")]
        public string Address { get; set; } = string.Empty;

        [Column("account_level")]
        [Required]
        public AccountLevelEnum AccountLevel { get; set; } = AccountLevelEnum.NORMAL;

        [Column("purchased")]
        public bool Purchased { get; set; } = false;

        // relationship
        // 1-1 Account
        public Account? Account { get; set; }
    }
}