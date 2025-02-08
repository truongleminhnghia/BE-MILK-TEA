using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    [Table("employee")]
    public class Employee
    {
        [Key]
        [Column("employ_id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("ref_code", TypeName = "varchar(100)")]
        public string RefCode { get; set; } = string.Empty;
    }
}