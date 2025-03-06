using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access_Layer.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientQuantities_ingredient_ingredient_id",
                table: "IngredientQuantities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientQuantities",
                table: "IngredientQuantities");

            migrationBuilder.RenameTable(
                name: "IngredientQuantities",
                newName: "ingredient_quantity");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientQuantities_ingredient_id",
                table: "ingredient_quantity",
                newName: "IX_ingredient_quantity_ingredient_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "create_at",
                table: "ingredient_quantity",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "update_at",
                table: "ingredient_quantity",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ingredient_quantity",
                table: "ingredient_quantity",
                column: "ingredient_quantity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredient_quantity_ingredient_ingredient_id",
                table: "ingredient_quantity",
                column: "ingredient_id",
                principalTable: "ingredient",
                principalColumn: "ingredient_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredient_quantity_ingredient_ingredient_id",
                table: "ingredient_quantity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ingredient_quantity",
                table: "ingredient_quantity");

            migrationBuilder.DropColumn(
                name: "create_at",
                table: "ingredient_quantity");

            migrationBuilder.DropColumn(
                name: "update_at",
                table: "ingredient_quantity");

            migrationBuilder.RenameTable(
                name: "ingredient_quantity",
                newName: "IngredientQuantities");

            migrationBuilder.RenameIndex(
                name: "IX_ingredient_quantity_ingredient_id",
                table: "IngredientQuantities",
                newName: "IX_IngredientQuantities_ingredient_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientQuantities",
                table: "IngredientQuantities",
                column: "ingredient_quantity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientQuantities_ingredient_ingredient_id",
                table: "IngredientQuantities",
                column: "ingredient_id",
                principalTable: "ingredient",
                principalColumn: "ingredient_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
