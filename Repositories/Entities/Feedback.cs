using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    [Table("feedback")]
    public class Feedback : BaseEntity
    {
        [Key]
        [Column("feedback_id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("comment", TypeName = "nvarchar(500 )")]
        public string Comment { get; set; } = string.Empty;

        [Column("order_id")]
        [Required(ErrorMessage = " order id must be note null")]
        public Guid OrderId { get; set; }

        public Order? Order {get; set;}

        [Column("account_id")]
        [Required(ErrorMessage = "account id must be non null")]
        public Guid AccountId { get; set; }

        public Account Account { get; set; }

        // constructor
        public Feedback()
        {
        }
        public Feedback(string comment, Guid orderId, Order? order, Guid accountId)
        {
            Comment = comment;
            OrderId = orderId;
            AccountId = accountId;
        }
    }
}