using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_column_name_position_teacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Teachers_HeadDepartment_Teacher_Id",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Facultys_Teachers_Dean_Teacher_Id",
                table: "Facultys");

            migrationBuilder.DropIndex(
                name: "IX_Facultys_Dean_Teacher_Id",
                table: "Facultys");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadDepartment_Teacher_Id",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "Dean_Teacher_Id",
                table: "Facultys",
                newName: "Dean_TeacherId");

            migrationBuilder.RenameColumn(
                name: "HeadDepartment_Teacher_Id",
                table: "Departments",
                newName: "HeadDepartment_TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Facultys_Dean_TeacherId",
                table: "Facultys",
                column: "Dean_TeacherId",
                unique: true,
                filter: "[Dean_TeacherId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadDepartment_TeacherId",
                table: "Departments",
                column: "HeadDepartment_TeacherId",
                unique: true,
                filter: "[HeadDepartment_TeacherId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Teachers_HeadDepartment_TeacherId",
                table: "Departments",
                column: "HeadDepartment_TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facultys_Teachers_Dean_TeacherId",
                table: "Facultys",
                column: "Dean_TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Teachers_HeadDepartment_TeacherId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Facultys_Teachers_Dean_TeacherId",
                table: "Facultys");

            migrationBuilder.DropIndex(
                name: "IX_Facultys_Dean_TeacherId",
                table: "Facultys");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadDepartment_TeacherId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "Dean_TeacherId",
                table: "Facultys",
                newName: "Dean_Teacher_Id");

            migrationBuilder.RenameColumn(
                name: "HeadDepartment_TeacherId",
                table: "Departments",
                newName: "HeadDepartment_Teacher_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Facultys_Dean_Teacher_Id",
                table: "Facultys",
                column: "Dean_Teacher_Id",
                unique: true,
                filter: "[Dean_Teacher_Id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadDepartment_Teacher_Id",
                table: "Departments",
                column: "HeadDepartment_Teacher_Id",
                unique: true,
                filter: "[HeadDepartment_Teacher_Id] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Teachers_HeadDepartment_Teacher_Id",
                table: "Departments",
                column: "HeadDepartment_Teacher_Id",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facultys_Teachers_Dean_Teacher_Id",
                table: "Facultys",
                column: "Dean_Teacher_Id",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
