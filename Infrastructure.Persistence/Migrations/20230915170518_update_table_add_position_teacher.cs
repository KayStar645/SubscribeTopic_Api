using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_add_position_teacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Teachers",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Dean_Teacher_Id",
                table: "Facultys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadDepartment_Teacher_Id",
                table: "Departments",
                type: "int",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Dean_Teacher_Id",
                table: "Facultys");

            migrationBuilder.DropColumn(
                name: "HeadDepartment_Teacher_Id",
                table: "Departments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Teachers",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }
    }
}
