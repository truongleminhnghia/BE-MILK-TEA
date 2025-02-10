using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Enums;

namespace Repositories.Entities
{
    [Table("order")]
    public class Order
    {
        [Key]
        [Column("order_ID")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("account_id")]
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        [Column("ref_code", TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "ref_code must be not null")]
        public string RefCode { get; set; }

        [Column("order_code")]
        [Required(ErrorMessage = "Order code must be not null")]
        public string OrderCode { get; set; }

        [Column("order_date", TypeName = "datetime")]
        [Required(ErrorMessage = "Order date must be not null")]
        public DateTime OrderDate { get; set; }

        [Column("full_name_shinpping", TypeName = "nvarchar(300)")]
        public string FullNameShipping { get; set; } = string.Empty;

        [Column("address_shipping")]
        public string AddressShipping { get; set; } = string.Empty;

        [Column("phone_shipping")]
        public string PhoneShpping { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("order_status")]
        [Required(ErrorMessage = "Order status must be not null")]
        public OrderStatus OrderStatus { get; set; }

        [Column("quantity")]
        [Required(ErrorMessage = "Quantity cannot be less than 0")]
        public int Quantity { get; set; }

        [Column("total_amount")]
        [Required(ErrorMessage = "Total amount cannot be less than 0")]
        public double TotalAmount { get; set; }

        [Column("price_promotion")]
        public double PricePromotion { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<OrderPromotion> OrdersPromotions { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        // Constructor
        public Order()
        {
        }
        public Order(Guid accountId, string refCode, string orderCode, DateTime orderDate, string fullNameShipping, string addressShipping, string phoneShpping, string email, OrderStatus orderStatus, int quantity, double totalAmount, double pricePromotion)
        {
            AccountId = accountId;
            RefCode = refCode;
            OrderCode = orderCode;
            OrderDate = orderDate;
            FullNameShipping = fullNameShipping;
            AddressShipping = addressShipping;
            PhoneShpping = phoneShpping;
            Email = email;
            OrderStatus = orderStatus;
            Quantity = quantity;
            TotalAmount = totalAmount;
            PricePromotion = pricePromotion;
        }

    }
}