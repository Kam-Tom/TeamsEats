using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamsEats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SaveCurrentPricesInOrderAndItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentDeliveryFee",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDeliveryFee",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "items");
        }
    }
}
