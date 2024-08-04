using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAE_API.Migrations
{
    /// <inheritdoc />
    public partial class _05032024_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Debtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerIdentification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDebt = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Debt = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Summation = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedDeliveryNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateddDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateddBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debtors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtorPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateddDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateddBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebtorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtorPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtorPayment_Debtors_DebtorId",
                        column: x => x.DebtorId,
                        principalTable: "Debtors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtorPayment_DebtorId",
                table: "DebtorPayment",
                column: "DebtorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtorPayment");

            migrationBuilder.DropTable(
                name: "Debtors");
        }
    }
}
