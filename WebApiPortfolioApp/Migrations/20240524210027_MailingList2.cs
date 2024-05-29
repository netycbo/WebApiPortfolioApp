using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class MailingList2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribedToMailingList",
                table: "SearchHistory");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedToLowBeerPriceAletr",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribedToLowBeerPriceAletr",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedToMailingList",
                table: "SearchHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
