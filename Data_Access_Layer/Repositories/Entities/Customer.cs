using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("customer")]
    public class Customer : BaseEntity
    {
        [Key]
        [Column("customer_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("account_id")]
        [Required]
        [ForeignKey("account_id")]
        public Guid AccountId { get; set; }

        [Column("tax_code", TypeName = "varchar(200)")]
        public string TaxCode { get; set; } = string.Empty;

        // relationship
        // 1-1 Account
        public Account? Account { get; set; }

        public Customer()
        {
            
        }
    }
}