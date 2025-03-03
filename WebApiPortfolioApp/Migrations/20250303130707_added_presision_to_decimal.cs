using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class added_presision_to_decimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<string>(
                name: "SearchString",
                table: "SearchHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchString",
                table: "SearchHistory",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_SearchString",
                table: "SearchHistory",
                column: "SearchString");
        }
    }
}
