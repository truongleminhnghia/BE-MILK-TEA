using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
{
    [Table("order_info")]
    public class Order
    {
        [Key]
        [Column("order_id", TypeName = "CHAR(36)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("order_code", TypeName = "varchar(200)")]
        [Required(ErrorMessage = "Order Code cannot be null")]
        public string? OrderCode { get; set; }

        [Column("order_date", TypeName = "datetime")]
        [Required(ErrorMessage = "Order Date cannot be null")]
        public DateTime OrderDate { get; set; }

        [Column("full_name_shipping", TypeName = "varchar(500)")]
        [Required(ErrorMessage = "Name cannot be null")]
        public string? FullNameShipping { get; set; }

        [Column("phone_shipping", TypeName = "varchar(15)")]
        [Required(ErrorMessage = "Phone cannot be null")]
        public string? PhoneShipping { get; set; }

        [Column("email_shipping", TypeName = "varchar(500)")]
        public string? EmailShipping { get; set; }

        [Column("note_shipping", TypeName = "varchar(500)")]
        public string? NoteShipping { get; set; }

        [Column("address_shipping", TypeName = "varchar(500)")]
        [Required(ErrorMessage = "Address cannot be null")]
        public string? AddressShipping { get; set; }

        [Column("order_status")]
        [Required(ErrorMessage = "Order status cannot be null")]
        public OrderStatus OrderStatus { get; set; }

        [Column("quantity")]
        [Required(ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Column("total_price")]
        [Required(ErrorMessage = "Totla price must be greater than 0")]
        public double TotalPrice { get; set; }

        [Column("price_affter_promotion")]
        public double? PriceAfterPromotion { get; set; }

        [Column("ref_code")]
        public string RefCode { get; set; } = string.Empty;

        [Column("reason_cancel", TypeName = "varchar(500)")]
        public string? ReasonCancel { get; set; }

        [Column("account_id")]
        [ForeignKey("AccountId")]
        [Required(ErrorMessage = "Account ID cannot be null")]
        public Guid AccountId { get; set; }

        // relationship 1-N with orderdetail
        // N-1 with account
        // N-N with promotion
        // 1-N with payment

        public Account? Account { get; set; }

        public ICollection<Payment>? Payments { get; set; }
        public ICollection<OrderPromotion>? OrderPromotions { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
