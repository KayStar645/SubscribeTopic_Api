using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_faculty_duty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "FacultyDuties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDuties_DepartmentId",
                table: "FacultyDuties",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyDuties_Departments_DepartmentId",
                table: "FacultyDuties",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacultyDuties_Departments_DepartmentId",
                table: "FacultyDuties");

            migrationBuilder.DropIndex(
                name: "IX_FacultyDuties_DepartmentId",
                table: "FacultyDuties");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "FacultyDuties");
        }
    }
}
