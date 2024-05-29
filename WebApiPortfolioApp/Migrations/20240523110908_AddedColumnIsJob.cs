using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnIsJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsJob",
                table: "SearchHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsJob",
                table: "SearchHistory");
        }
    }
}
