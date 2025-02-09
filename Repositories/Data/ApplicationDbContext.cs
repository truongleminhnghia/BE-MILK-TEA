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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientProduct> IngredientProducts { get; set; }
        public DbSet<IngredientPromotion> IngredientPromotions { get; set; }
        public DbSet<IngredientRecipe> IngredientRecipes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionDetail> PromotionDetails { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderPromotion> OrderPromotions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IngredientProduct>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.Products)
                .HasForeignKey(ip => ip.IngredientId);

            modelBuilder.Entity<IngredientPromotion>()
                .HasKey(ip => new { ip.IngredientId, ip.PromotionId });

            modelBuilder.Entity<IngredientPromotion>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.IngredientsPromotions)
                .HasForeignKey(ip => ip.IngredientId);

            modelBuilder.Entity<IngredientPromotion>()
                .HasOne(ip => ip.Promotion)
                .WithMany(p => p.IngredientPromotions)
                .HasForeignKey(ip => ip.PromotionId);

            modelBuilder.Entity<IngredientRecipe>()
                .HasKey(ir => new { ir.IngredientId, ir.RecipeId });

            modelBuilder.Entity<IngredientRecipe>()
                .HasOne(ir => ir.Ingredient)
                .WithMany(i => i.IngredientRecipes)
                .HasForeignKey(ir => ir.IngredientId);

            modelBuilder.Entity<IngredientRecipe>()
                .HasOne(ir => ir.Recipe)
                .WithMany(r => r.IngredientsRecipes)
                .HasForeignKey(ir => ir.RecipeId);

            modelBuilder.Entity<PromotionDetail>()
                .HasOne(pd => pd.Promotion)
                .WithOne(p => p.PromotionDetail)
                .HasForeignKey<PromotionDetail>(pd => pd.PromotionId);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Ingredient)
                .WithMany(ing => ing.Images)
                .HasForeignKey(i => i.IngredientId);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Ingredients)
                .HasForeignKey(i => i.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.IngredientProduct)
                .WithMany(ip => ip.OrderDetails)
                .HasForeignKey(od => od.IngredientProductId);

            modelBuilder.Entity<OrderPromotion>()
                .HasKey(op => new { op.OrderId, op.PromotionId });

            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrdersPromotions)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderPromotion>()
                .HasOne(op => op.Promotion)
                .WithMany(p => p.OrderPromotions)
                .HasForeignKey(op => op.PromotionId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Cart)
                .WithOne(c => c.Account)
                .HasForeignKey<Cart>(c => c.AccountId)
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(ip => ip.CartItems)
                .HasForeignKey(ci => ci.ProductId);

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
