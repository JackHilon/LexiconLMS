using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Data.Migrations
{
    public partial class UserCreateInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_AppUserId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_ModuleActivity_ModuleActivityId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.DropIndex(
                name: "IX_ModuleActivity_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "ModuleActivity");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ModuleActivityId",
                table: "Documents",
                newName: "IX_Documents_ModuleActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_AppUserId",
                table: "Documents",
                newName: "IX_Documents_AppUserId");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId1",
                table: "ModuleActivity",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActivity_ModuleId1",
                table: "ModuleActivity",
                column: "ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_AppUserId",
                table: "Documents",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ModuleActivity_ModuleActivityId",
                table: "Documents",
                column: "ModuleActivityId",
                principalTable: "ModuleActivity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId1",
                table: "ModuleActivity",
                column: "ModuleId1",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_AppUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ModuleActivity_ModuleActivityId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.DropIndex(
                name: "IX_ModuleActivity_ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ModuleActivityId",
                table: "Document",
                newName: "IX_Document_ModuleActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_AppUserId",
                table: "Document",
                newName: "IX_Document_AppUserId");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "ModuleActivity",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActivity_ModuleId",
                table: "ModuleActivity",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_AppUserId",
                table: "Document",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_ModuleActivity_ModuleActivityId",
                table: "Document",
                column: "ModuleActivityId",
                principalTable: "ModuleActivity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
