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
        public DbSet<Ingredient_Product> Ingredients_Products { get; set; }
        public DbSet<Ingredient_Promotion> Ingredients_Promotions { get; set; }
        public DbSet<Ingredient_Recipe> Ingredients_Recipes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Promotion_Detail> Promotion_Details { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Image> Images { get; set; }



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

            modelBuilder.Entity<Ingredient_Product>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.Products)
                .HasForeignKey(ip => ip.Ingredient_Id);

            modelBuilder.Entity<Ingredient_Promotion>()
                .HasKey(ip => new { ip.Ingredient_Id, ip.Promotion_Id });

            modelBuilder.Entity<Ingredient_Promotion>()
                .HasOne(ip => ip.Ingredient)
                .WithMany(i => i.Ingredients_Promotions)
                .HasForeignKey(ip => ip.Ingredient_Id);

            modelBuilder.Entity<Ingredient_Promotion>()
                .HasOne(ip => ip.Promotion)
                .WithMany(p => p.Ingredients_Promotions)
                .HasForeignKey(ip => ip.Promotion_Id);

            modelBuilder.Entity<Ingredient_Recipe>()
                .HasKey(ir => new { ir.Ingredient_Id, ir.Recipe_Id });

            modelBuilder.Entity<Ingredient_Recipe>()
                .HasOne(ir => ir.Ingredient)
                .WithMany(i => i.Ingredient_Recipes)
                .HasForeignKey(ir => ir.Ingredient_Id);

            modelBuilder.Entity<Ingredient_Recipe>()
                .HasOne(ir => ir.Recipe)
                .WithMany(r => r.Ingredients_Recipes)
                .HasForeignKey(ir => ir.Recipe_Id);

            modelBuilder.Entity<Promotion_Detail>()
                .HasOne(pd => pd.Promotion)
                .WithOne(p => p.Promotion_Detail)
                .HasForeignKey<Promotion_Detail>(pd => pd.Promotion_Id);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Ingredient)
                .WithMany(ing => ing.Images)
                .HasForeignKey(i => i.Ingredient_Id);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.Category_Id);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Ingredients)
                .HasForeignKey(i => i.Category_Id);
        }
    }
}
