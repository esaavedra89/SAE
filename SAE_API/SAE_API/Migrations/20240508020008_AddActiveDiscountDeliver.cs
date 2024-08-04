using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAE_API.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveDiscountDeliver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ItemDeliveryNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "DeliveryNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deliver",
                table: "DeliveryNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "ItemDeliveryNotes");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "DeliveryNotes");

            migrationBuilder.DropColumn(
                name: "Deliver",
                table: "DeliveryNotes");
        }
    }
}
