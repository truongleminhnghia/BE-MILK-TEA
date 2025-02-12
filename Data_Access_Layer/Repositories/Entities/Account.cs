using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("account")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("email", TypeName = "nvarchar(300)")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Column("last_name", TypeName = "nvarchar(300)")]
        public string LastName { get; set; } = string.Empty;

        [Column("first_name", TypeName = "nvarchar(300)")]
        public string FirstName { get; set; } = string.Empty;

        [Column("password", TypeName = "nvarchar(200)")]
        public string Password { get; set; } = string.Empty;

        [Column("account_status", TypeName = "varchar(50)")]
        [Required]
        [EnumDataType(typeof(AccountStatus))]
        public AccountStatus AccountStatus { get; set; }

        [Column("phone_number", TypeName = "varchar(15)")]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Column("role_name", TypeName = "varchar(200)")]
        [Required]
        [EnumDataType(typeof(RoleName))]
        public RoleName RoleName { get; set; }

        // relationship
        // 1-1 with cart
        // 1-1 Customer
        // 1-1 Employee
        // 1-N Order
        // 1-N IngredientReview

        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<IngredientReview>? IngredientReviews { get; set; }

        public Customer? Customer { get; set; }
        public Empoyee? Empoyee { get; set; }
        public Cart? Cart { get; set; }

        public Account()
        {
            
        }
    }
}

// đk, đn, change pass, edit CRUD <oauth2> GG FB
// đk - GG, FB // trả về FE một link, (url, url return)
// URL FE: localhost:5170
// DK - LOCAL (Email) --> xác thực OPT
// LOCAL -> tạo account (STAUT đang chờ xác thực), nhập OTP thsanhf công thì --- active
// Mua -> check status