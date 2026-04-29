using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusToOrderSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "OrderSummaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "OrderSummaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "OrderSummaries");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "OrderSummaries");
        }
    }
}
