using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class ProductSubscriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubscribedToLowBeerPriceAletr",
                table: "AspNetUsers",
                newName: "IsSubscribedToNewsLetter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubscribedToNewsLetter",
                table: "AspNetUsers",
                newName: "IsSubscribedToLowBeerPriceAletr");
        }
    }
}
