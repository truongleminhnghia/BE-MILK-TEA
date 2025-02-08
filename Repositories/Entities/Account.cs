using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Enums;

namespace Repositories.Entities
{
    public class Account : BaseEntity
    {
        [Key]
        [Column("account_id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("first_name", TypeName = "nvarchar(300)")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name", TypeName = "nvarchar(300)")]
        public string LastName { get; set; } = string.Empty;

        [Column("email")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Column("password")]
        public string Pasword { get; set; } = string.Empty;

        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("account_status", TypeName = "Enum")]
        [Required(ErrorMessage = "Account status is required")]
        public AccountStatus AccountStatus { get; set; }

        [Column("role_name")]
        [Required(ErrorMessage = "Role is required")]
        public RoleNameEnum Role { get; set; } = RoleNameEnum.ROLE_CUSTOMER;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
