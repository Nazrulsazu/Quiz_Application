using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz_App.Data.Migrations
{
    public partial class ScriptH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_AuthorId",
                table: "Templates");

            migrationBuilder.CreateTable(
                name: "TemplateLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateLikes_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLikes_TemplateId",
                table: "TemplateLikes",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLikes_UserId",
                table: "TemplateLikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_AuthorId",
                table: "Templates",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_AuthorId",
                table: "Templates");

            migrationBuilder.DropTable(
                name: "TemplateLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_AuthorId",
                table: "Templates",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
