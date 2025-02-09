using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Account_Recipe> Accounts_Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientProduct> Ingredients_Products { get; set; }
        public DbSet<IngredientPromotion> Ingredients_Promotions { get; set; }
        public DbSet<IngredientRecipe> Ingredients_Recipes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionDetail> Promotion_Details { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> Order_Details { get; set; }
        public DbSet<OrderPromotion> Orders_Promotions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> Cart_Items { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account_Recipe>()
                .HasKey(ar => new { ar.Account_Id, ar.Recipe_Id });

            modelBuilder.Entity<Account_Recipe>()
                .HasOne(ar => ar.Account)
                .WithMany(a => a.Accounts_Recipes)
                .HasForeignKey(ar => ar.Account_Id);

            modelBuilder.Entity<Account_Recipe>()
                .HasOne(ar => ar.Recipe)
                .WithMany(r => r.Accounts_Recipes)
                .HasForeignKey(ar => ar.Recipe_Id);

            modelBuilder.Entity<IngredientProduct>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.Products)
                .HasForeignKey(ip => ip.Ingredient_Id);

            modelBuilder.Entity<IngredientPromotion>()
                .HasKey(ip => new { ip.Ingredient_Id, ip.Promotion_Id });

            modelBuilder.Entity<IngredientPromotion>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.Ingredients_Promotions)
                .HasForeignKey(ip => ip.Ingredient_Id);

            modelBuilder.Entity<IngredientPromotion>()
                .HasOne(ip => ip.Promotion)
                .WithMany(p => p.Ingredients_Promotions)
                .HasForeignKey(ip => ip.Promotion_Id);

            modelBuilder.Entity<IngredientRecipe>()
                .HasKey(ir => new { ir.Ingredient_Id, ir.Recipe_Id });

            modelBuilder.Entity<IngredientRecipe>()
                .HasOne(ir => ir.Ingredient)
                .WithMany(i => i.Ingredient_Recipes)
                .HasForeignKey(ir => ir.Ingredient_Id);

            modelBuilder.Entity<IngredientRecipe>()
                .HasOne(ir => ir.Recipe)
                .WithMany(r => r.Ingredients_Recipes)
                .HasForeignKey(ir => ir.Recipe_Id);

            modelBuilder.Entity<PromotionDetail>()
                .HasOne(pd => pd.Promotion)
                .WithOne(p => p.Promotion_Detail)
                .HasForeignKey<PromotionDetail>(pd => pd.Promotion_Id);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Ingredient)
                .WithMany(ing => ing.Images)
                .HasForeignKey(i => i.Ingredient_Id);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Ingredients)
                .HasForeignKey(i => i.Category_Id);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Ingredient_Product)
                .WithMany(ip => ip.Order_Details)
                .HasForeignKey(od => od.Product_Id);

            modelBuilder.Entity<OrderPromotion>()
                .HasKey(op => new { op.Order_Id, op.Promotion_Id });

            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderDetails_Promotion)
                .HasForeignKey(op => op.Order_Id);

            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Promotion)
                .WithMany(p => p.Orders_Promotions)
                .HasForeignKey(op => op.Promotion_Id);

            //cart
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Cart)  // Một Account có một Cart
                .WithOne(c => c.Account)  // Một Cart thuộc về một Account
                .HasForeignKey<Cart>(c => c.AccountId)  // Cart chứa khóa ngoại AccountId
                .IsRequired(); // Đảm bảo mỗi Cart phải có một Account

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Cart_Items)
                .HasForeignKey(ci => ci.Cart_Id);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(ip => ip.Cart_Items)
                .HasForeignKey(ci => ci.Product_Id);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Account)
                .WithMany(a => a.Feedbacks)
                .HasForeignKey(f => f.AccountId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Order)
                .WithMany(o => o.Feedbacks)
                .HasForeignKey(f => f.OrderId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Customers)
                .HasForeignKey(c => c.AccountId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Account)
                .WithMany(a => a.Employees)
                .HasForeignKey(e => e.AccountId);
        }
    }

}
