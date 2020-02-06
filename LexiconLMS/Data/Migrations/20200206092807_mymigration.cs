using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Data.Migrations
{
    public partial class mymigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.DropIndex(
                name: "IX_ModuleActivity_ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "ModuleActivity");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "ModuleActivity",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActivity_ModuleId",
                table: "ModuleActivity",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.DropIndex(
                name: "IX_ModuleActivity_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "ModuleActivity");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId1",
                table: "ModuleActivity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActivity_ModuleId1",
                table: "ModuleActivity",
                column: "ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId1",
                table: "ModuleActivity",
                column: "ModuleId1",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
