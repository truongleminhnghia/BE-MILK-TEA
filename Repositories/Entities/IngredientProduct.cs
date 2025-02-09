using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Enums;

namespace Repositories.Entities
{
    public class IngredientProduct
    {
        [Key]
        [Column("ingredient_product_id")]
        public Guid IngredientProductId { get; set; }

        [Column("total_price")]
        public int TotalPrice { get; set; }

        [Column("is_sale")]
        public bool IsSale { get; set; } = false;

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit")]
        public ProductTypeEnum Unit { get; set; }

        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Constructor
        public IngredientProduct(int totalPrice, bool isSale, int quantity, ProductTypeEnum unit, Guid ingredientId)
        {
            IngredientProductId = Guid.NewGuid();
            TotalPrice = totalPrice;
            IsSale = isSale;
            Quantity = quantity;
            Unit = unit;
            IngredientId = ingredientId;
        }
    }
}
