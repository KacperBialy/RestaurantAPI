using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantAPI.Migrations
{
    public partial class RestaurantCreatetById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Restaurant",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_CreatedById",
                table: "Restaurant",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Users_CreatedById",
                table: "Restaurant",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Users_CreatedById",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_CreatedById",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Restaurant");
        }
    }
}
