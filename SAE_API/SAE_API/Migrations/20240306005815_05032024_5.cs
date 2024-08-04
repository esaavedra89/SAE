using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAE_API.Migrations
{
    /// <inheritdoc />
    public partial class _05032024_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebtorPayment_Debtors_DebtorId",
                table: "DebtorPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DebtorPayment",
                table: "DebtorPayment");

            migrationBuilder.DropIndex(
                name: "IX_DebtorPayment_DebtorId",
                table: "DebtorPayment");

            migrationBuilder.RenameTable(
                name: "DebtorPayment",
                newName: "DebtorPayments");

            migrationBuilder.AlterColumn<int>(
                name: "DebtorId",
                table: "DebtorPayments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DebtorPayments",
                table: "DebtorPayments",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DebtorPayments",
                table: "DebtorPayments");

            migrationBuilder.RenameTable(
                name: "DebtorPayments",
                newName: "DebtorPayment");

            migrationBuilder.AlterColumn<int>(
                name: "DebtorId",
                table: "DebtorPayment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DebtorPayment",
                table: "DebtorPayment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DebtorPayment_DebtorId",
                table: "DebtorPayment",
                column: "DebtorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DebtorPayment_Debtors_DebtorId",
                table: "DebtorPayment",
                column: "DebtorId",
                principalTable: "Debtors",
                principalColumn: "Id");
        }
    }
}
