using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("account_id")]
        public Guid AccountId { get; set; }

        [Column("address", TypeName = "nvarchar(500)")]
        public string Address { get; set; } = string.Empty;

        [Column("tax_code", TypeName = "varchar(500)")]
        public string TaxCode { get; set; } = string.Empty;

        public Account Account { get; set; }

    }
}