
using System;
using Data_Access_Layer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data_Access_Layer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Data_Access_Layer.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<string>("AccountStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("account_status");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("first_name");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("image_url");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("varchar(15)")
                        .HasColumnName("phone_number");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("role_name");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("account");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("cart_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("cart");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("cart_item_id");

                    b.Property<Guid>("CartId")
                        .HasColumnType("char(36)")
                        .HasColumnName("cart_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsCart")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("isCart");

                    b.Property<double?>("Price")
                        .HasColumnType("double")
                        .HasColumnName("price");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("product_type");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<double?>("TotalPrice")
                        .HasColumnType("double")
                        .HasColumnName("total_price");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("IngredientId");

                    b.ToTable("cart_item");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("category_id");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("category_name");

                    b.Property<string>("CategoryStatus")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("category_status");

                    b.Property<string>("CategoryType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("category_type");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.ToTable("category");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("customer_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<int>("AccountLevel")
                        .HasColumnType("int")
                        .HasColumnName("account_level");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("address");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<bool>("Purchased")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("purchased");

                    b.Property<string>("TaxCode")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("tax_code");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("customer");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("empoyee_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("ref_code");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("employee");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("image_id");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("image_url");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("image");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Ingredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("char(36)")
                        .HasColumnName("category_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("description");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("datetime")
                        .HasColumnName("expired_date");

                    b.Property<string>("FoodSafetyCertification")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("food_safety_certification");

                    b.Property<string>("IngredientCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ingredient_code");

                    b.Property<string>("IngredientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("ingredient_name");

                    b.Property<string>("IngredientStatus")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ingredient_status");

                    b.Property<string>("IngredientType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ingredient_type");

                    b.Property<bool>("IsSale")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_sale");

                    b.Property<double>("PriceOrigin")
                        .HasColumnType("double")
                        .HasColumnName("price_origin");

                    b.Property<double>("PricePromotion")
                        .HasColumnType("double")
                        .HasColumnName("price_promotion");

                    b.Property<int>("QuantityPerCarton")
                        .HasColumnType("int")
                        .HasColumnName("quantity_per_carton");

                    b.Property<float>("Rate")
                        .HasColumnType("float")
                        .HasColumnName("rate");

                    b.Property<string>("Supplier")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("supplier");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("unit");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.Property<float>("WeightPerBag")
                        .HasColumnType("float")
                        .HasColumnName("weight_per_bag");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("ingredient");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientProduct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_product_id");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("product_type");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("double")
                        .HasColumnName("total_price");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("ingredient_product");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientPromotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_promotion_id");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("char(36)")
                        .HasColumnName("promotion_id");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("PromotionId");

                    b.ToTable("ingredient_promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientQuantity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_quantity_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("product_type");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("ingredient_quantity");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientRecipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_recipe_id");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("char(36)")
                        .HasColumnName("recipe_id");

                    b.Property<float>("WeightOfIngredient")
                        .HasColumnType("float")
                        .HasColumnName("weight_of_ingredient");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("RecipeId");

                    b.ToTable("ingredient_recipe");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientReview", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("feedback_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("comment");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("char(36)")
                        .HasColumnName("ingredient_id");

                    b.Property<double>("Rate")
                        .HasColumnType("double")
                        .HasColumnName("rate");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("IngredientId");

                    b.ToTable("ingredient_review");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("order_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)")
                        .HasColumnName("account_id");

                    b.Property<string>("AddressShipping")
                        .IsRequired()
                        .HasColumnType("varchar(500)")
                        .HasColumnName("address_shipping");

                    b.Property<string>("EmailShipping")
                        .HasColumnType("varchar(500)")
                        .HasColumnName("email_shipping");

                    b.Property<string>("FullNameShipping")
                        .IsRequired()
                        .HasColumnType("varchar(500)")
                        .HasColumnName("full_name_shipping");

                    b.Property<string>("NoteShipping")
                        .HasColumnType("varchar(500)")
                        .HasColumnName("note_shipping");

                    b.Property<string>("OrderCode")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("order_code");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime")
                        .HasColumnName("order_date");

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("order_status");

                    b.Property<string>("PhoneShipping")
                        .IsRequired()
                        .HasColumnType("varchar(15)")
                        .HasColumnName("phone_shipping");

                    b.Property<double?>("PriceAffterPromotion")
                        .HasColumnType("double")
                        .HasColumnName("price_affter_promotion");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<string>("ReasonCancel")
                        .HasColumnType("varchar(500)")
                        .HasColumnName("reason_cancel");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ref_code");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("double")
                        .HasColumnName("total_price");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("order_info");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("order_detail_id");

                    b.Property<Guid>("CartItemId")
                        .HasColumnType("char(36)")
                        .HasColumnName("cart_item_id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)")
                        .HasColumnName("order_id");

                    b.Property<double>("Price")
                        .HasColumnType("double")
                        .HasColumnName("price");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.HasKey("Id");

                    b.HasIndex("CartItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("order_detail");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.OrderPromotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("order_promotion_id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)")
                        .HasColumnName("order_id");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("char(36)")
                        .HasColumnName("promotion_id");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("PromotionId");

                    b.ToTable("order_promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("payment_id");

                    b.Property<double>("AmountPaid")
                        .HasColumnType("double")
                        .HasColumnName("amount_paid");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)")
                        .HasColumnName("order_id");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime")
                        .HasColumnName("payment_date");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("payment_method");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("payment_status");

                    b.Property<double>("RemainingAmount")
                        .HasColumnType("double")
                        .HasColumnName("remaining_amount");

                    b.Property<double>("TotlaPrice")
                        .HasColumnType("double")
                        .HasColumnName("total_price");

                    b.Property<string>("TranscationId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("transcation_id");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("payment");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Promotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("promotion_id");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("end_date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("PromotionCode")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("promotion_code");

                    b.Property<string>("PromotionType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("promotion_type");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime")
                        .HasColumnName("start_date");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.ToTable("promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.PromotionDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("promotion_detail_id");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("description");

                    b.Property<float>("DiscountValue")
                        .HasColumnType("float")
                        .HasColumnName("discount_value");

                    b.Property<double>("MaxValue")
                        .HasColumnType("double")
                        .HasColumnName("max_value");

                    b.Property<double>("MiniValue")
                        .HasColumnType("double")
                        .HasColumnName("mini_value");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("char(36)")
                        .HasColumnName("promtion_id");

                    b.Property<string>("PromotionName")
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("promotion_name");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId")
                        .IsUnique();

                    b.ToTable("promotion_detail");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("recipe_id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("char(36)")
                        .HasColumnName("category_id");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(2000)")
                        .HasColumnName("content");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_at");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("image_url");
                    b.Property<string>("RecipeLevel")
                        .IsRequired()
                        .HasColumnType("longtext")

                        .HasColumnName("recipe_level");

                    b.Property<int>("RecipeStatus")
                        .HasColumnType("int")
                        .HasColumnName("recipe_status");

                    b.Property<string>("RecipeTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("recipe_title");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_at");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("recipe");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("id");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expiration_date");

                    b.Property<string>("TokenString")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("token");

                    b.HasKey("Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Cart", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Account", "Account")
                        .WithOne("Cart")
                        .HasForeignKey("Data_Access_Layer.Entities.Cart", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.CartItem", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("CartItems")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Customer", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Account", "Account")
                        .WithOne("Customer")
                        .HasForeignKey("Data_Access_Layer.Entities.Customer", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Employee", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Account", "Account")
                        .WithOne("Employee")
                        .HasForeignKey("Data_Access_Layer.Entities.Employee", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Image", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("Images")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Ingredient", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Category", "Category")
                        .WithMany("Ingredients")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientProduct", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("IngredientProducts")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientPromotion", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("IngredientPromotions")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Promotion", "Promotion")
                        .WithMany("IngredientPromotions")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientQuantity", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredients")
                        .WithMany("IngredientQuantities")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientRecipe", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("IngredientRecipes")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Recipe", "Recipe")
                        .WithMany("IngredientRecipes")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.IngredientReview", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Account", "Account")
                        .WithMany("IngredientReviews")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Ingredient", "Ingredient")
                        .WithMany("IngredientReviews")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Order", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Account", "Account")
                        .WithMany("Orders")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.OrderDetail", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.CartItem", "CartItems")
                        .WithMany()
                        .HasForeignKey("CartItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Order", "Orders")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CartItems");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.OrderPromotion", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Order", "Order")
                        .WithMany("OrderPromotions")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data_Access_Layer.Entities.Promotion", "Promotion")
                        .WithMany("OrderPromotions")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Payment", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Order", "Order")
                        .WithMany("Payments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.PromotionDetail", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Promotion", "Promotion")
                        .WithOne("PromotionDetail")
                        .HasForeignKey("Data_Access_Layer.Entities.PromotionDetail", "PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Recipe", b =>
                {
                    b.HasOne("Data_Access_Layer.Entities.Category", "Category")
                        .WithMany("Recipes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Account", b =>
                {
                    b.Navigation("Cart");

                    b.Navigation("Customer");

                    b.Navigation("Employee");

                    b.Navigation("IngredientReviews");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Category", b =>
                {
                    b.Navigation("Ingredients");

                    b.Navigation("Recipes");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Ingredient", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Images");

                    b.Navigation("IngredientProducts");

                    b.Navigation("IngredientPromotions");

                    b.Navigation("IngredientQuantities");

                    b.Navigation("IngredientRecipes");

                    b.Navigation("IngredientReviews");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("OrderPromotions");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Promotion", b =>
                {
                    b.Navigation("IngredientPromotions");

                    b.Navigation("OrderPromotions");

                    b.Navigation("PromotionDetail");
                });

            modelBuilder.Entity("Data_Access_Layer.Entities.Recipe", b =>
                {
                    b.Navigation("IngredientRecipes");
                });
#pragma warning restore 612, 618
        }
    }
}
