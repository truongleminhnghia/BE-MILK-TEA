using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("empoyee")]
    public class Employee : BaseEntity
    {
        [Key]
        [Column("empoyee_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("account_id")]
        [Required]
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        [Column("ref_code", TypeName = "varchar(200)")]
        [Required]
        public string RefCode { get; set; } = string.Empty; // 66666666 số

        // relationship
        // 1-1 Account
        public Account? Account { get; set; }
    }
}