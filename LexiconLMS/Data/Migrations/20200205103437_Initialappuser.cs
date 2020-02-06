using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Data.Migrations
{
    public partial class Initialappuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentDescription = table.Column<string>(nullable: true),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    ModuleActivityId = table.Column<int>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Document_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_ModuleActivity_ModuleActivityId",
                        column: x => x.ModuleActivityId,
                        principalTable: "ModuleActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_AppUserId",
                table: "Document",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ModuleActivityId",
                table: "Document",
                column: "ModuleActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document");
        }
    }
}
