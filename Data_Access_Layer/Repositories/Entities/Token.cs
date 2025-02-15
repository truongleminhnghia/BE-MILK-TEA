using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    public class Token
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("token")]
        [Required]
        public string TokenString { get; set; } = string.Empty;

        [Column("expiration_date")]
        [Required]
        public DateTime ExpirationDate { get; set; }

    }
}
