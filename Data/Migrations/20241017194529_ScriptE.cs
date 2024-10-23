using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz_App.Data.Migrations
{
    public partial class ScriptE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Option",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Option_ApplicationUserId",
                table: "Option",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Option_AspNetUsers_ApplicationUserId",
                table: "Option",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Option_AspNetUsers_ApplicationUserId",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Option_ApplicationUserId",
                table: "Option");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Option");
        }
    }
}
