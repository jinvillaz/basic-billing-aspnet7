using System;
using BasicBilling.API.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicBilling.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Period = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    BillId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 100, "Joseph Carlton" },
                    { 200, "Maria Juarez" },
                    { 300, "Albert Kenny" },
                    { 400, "Jessica Phillips" },
                    { 500, "Charles Johnson" },
                });

            int[] months = { 202308, 202309 };
            int[] clientIds = { 100, 200, 300, 400, 500 };
            int[] categories = (int[])Enum.GetValues(typeof(BillCategory));

            foreach (int month in months)
            {
                foreach (int clientId in clientIds)
                {
                    foreach (int category in categories)
                    {
                        decimal randomAmount = Math.Round((decimal)new Random().NextDouble() * (200.0m - 100.0m) + 100.0m, 2);
                        migrationBuilder.InsertData(
                            table: "Bills",
                            columns: new[] { "ClientId", "Period", "Amount", "State", "Category" },
                            values: new object[] { clientId, month, randomAmount, (int)BillState.Pending, category });
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
