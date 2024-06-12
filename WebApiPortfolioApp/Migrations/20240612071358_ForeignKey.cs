using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPortfolioApp.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserIdId",
                table: "ProductSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserNameId",
                table: "ProductSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubscriptions_UserIdId",
                table: "ProductSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubscriptions_UserNameId",
                table: "ProductSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserIdId",
                table: "ProductSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserNameId",
                table: "ProductSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ProductSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_UserId",
                table: "ProductSubscriptions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserId",
                table: "ProductSubscriptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserId",
                table: "ProductSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubscriptions_UserId",
                table: "ProductSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ProductSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "UserIdId",
                table: "ProductSubscriptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameId",
                table: "ProductSubscriptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_UserIdId",
                table: "ProductSubscriptions",
                column: "UserIdId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_UserNameId",
                table: "ProductSubscriptions",
                column: "UserNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserIdId",
                table: "ProductSubscriptions",
                column: "UserIdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSubscriptions_AspNetUsers_UserNameId",
                table: "ProductSubscriptions",
                column: "UserNameId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
