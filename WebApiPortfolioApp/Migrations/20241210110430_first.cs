using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryPrices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfMaxPrice = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfMinPrice = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceDifference = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreWithMaxPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreWithMinPrice = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryPrices", x => x.Id);
                });
        }
    }
}
