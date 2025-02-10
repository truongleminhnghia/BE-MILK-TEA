using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Enums;

namespace Repositories.Entities
{
    [Table("payments")]
    public class Payment : BaseEntity
    {
        [Key]
        [Column("payment_id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("order_id")]
        [Required(ErrorMessage = " Order Id must be not null")]
        public Guid OrderId { get; set; }

        [Column("payment_method")]
        [Required(ErrorMessage = "Patment method must be not null")]
        public PaymentMethod PaymentMethod { get; set; }

        [Column("payment_status")]
        [Required(ErrorMessage = "Payment status method must be not null")]
        public PaymentStatus PaymentStatus { get; set; }

        [Column("transcation_id")]
        public string TranscationId { get; set; } = string.Empty;

        //           setup relationship
        public virtual Order Order { get; set; }

        // constructor
        public Payment()
        {
        }

        public Payment(Guid orderId, PaymentMethod paymentMethod, PaymentStatus paymentStatus, string transcationId)
        {
            OrderId = orderId;
            PaymentMethod = paymentMethod;
            PaymentStatus = paymentStatus;
            TranscationId = transcationId;
        }
    }
}