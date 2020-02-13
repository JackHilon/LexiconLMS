using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Migrations
{
    public partial class MyNewMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Related",
                table: "Documents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Related",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Documents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
