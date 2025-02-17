﻿using Data_Access_Layer.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Empoyees { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<IngredientProduct> IngredientProducts { get; set; }
        public virtual DbSet<IngredientPromotion> IngredientPromotions { get; set; }
        public virtual DbSet<IngredientRecipe> IngredientRecipes { get; set; }
        public virtual DbSet<IngredientReview> IngredientReviews { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderPromotion> OrderPromotions { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<PromotionDetail> PromotionDetails { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .Property(a => a.AccountStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Account>()
                .Property(a => a.RoleName)
                .HasConversion<string>();
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AccountId)
                .IsUnique();
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.AccountId)
                .IsUnique();
            modelBuilder.Entity<Cart>()
                .HasIndex(a => a.AccountId)
                .IsUnique();
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => ci.CartId)
                .IsUnique();         
            modelBuilder.Entity<Category>()
                .Property(a => a.CategoryStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Category>()
                .Property(a => a.CategoryType)
                .HasConversion<string>();
            modelBuilder.Entity<Ingredient>()
                .Property(a => a.IngredientStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Order>()
                .Property(a => a.OrderStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Payment>()
                .Property(a => a.PaymentMethod)
                .HasConversion<string>();
            modelBuilder.Entity<Payment>()
                .Property(a => a.PaymentStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Promotion>()
                .Property(a => a.PromotionType)
                .HasConversion<string>();

        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreateAt = DateTime.UtcNow;
                }
                entity.UpdateAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
    }
}
