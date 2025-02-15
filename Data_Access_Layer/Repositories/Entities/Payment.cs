using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("order_id")]
        [ForeignKey("OrderId")]
        [Required]
        public Guid OrderId { get; set; }

        [Column("payment_method")]
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Column("payment_date", TypeName = "datetime")]
        [Required]
        public DateTime PaymentDate { get; set; }

        [Column("payment_status")]
        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Column("transcation_id")]
        public string TranscationId { get; set; } = string.Empty;

        [Column("total_price")]
        [Required]
        public double TotlaPrice { get; set; }

        [Column("amount_paid")]
        public double AmountPaid { get; set; }

        [Column("remaining_amount")]
        public double RemainingAmount { get; set; }

        public Order? Order { get; set; }
    }
}