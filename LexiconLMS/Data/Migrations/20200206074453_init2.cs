using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Data.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "ModuleActivity",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivity_Module_ModuleId",
                table: "ModuleActivity");

            migrationBuilder.AlterColumn<int>(
                name: "ModuleId",
                table: "ModuleActivity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

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
