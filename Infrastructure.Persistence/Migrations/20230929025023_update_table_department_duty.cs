using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_department_duty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NumberOfThesis",
                table: "DepartmentDuties",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "DepartmentDuties",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDuties_Teachers_DepartmentId",
                table: "DepartmentDuties",
                column: "DepartmentId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDuties_Teachers_DepartmentId",
                table: "DepartmentDuties");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "DepartmentDuties");

            migrationBuilder.AlterColumn<string>(
                name: "NumberOfThesis",
                table: "DepartmentDuties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
