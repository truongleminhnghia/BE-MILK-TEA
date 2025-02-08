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
    public class Ingredient_Product
    {
        [Key]
        [Column ("ingredient_product_id")]
        public Guid Ingredient_Product_Id { get; set; } = Guid.NewGuid();

        [Column ("total_price")]
        public int Total_Price { get; set; }

        [Column("is_sale")]
        public bool Is_Sale { get; set; } = false;

        //amount of packages
        [Column("quantity")]
        public int Quantity { get; set; }

        //type of product: 1 bin = 12 packages
        [Column ("unit")]
        public ProductTypeEnum Unit { get; set; }

        [Column ("ingredient_id")]
        public Guid Ingredient_Id { get; set; }

        //      setup relationship
        public virtual Ingredient Ingredient { get; set; }

        public virtual ICollection<Cart_Item> Cart_Items { get; set; }
        public virtual ICollection<Order_Detail> Order_Details { get; set; }
    }
}
