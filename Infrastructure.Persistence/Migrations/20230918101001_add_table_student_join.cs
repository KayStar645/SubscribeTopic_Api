using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class add_table_student_join : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Semester",
                table: "RegistrationPeriods",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "RegistrationPeriods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "RegistrationPeriods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentJoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    Phase = table.Column<int>(type: "int", nullable: true),
                    Semester = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    isLeader = table.Column<bool>(type: "bit", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentJoins_RegistrationPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "RegistrationPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentJoins_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_FacultyId",
                table: "RegistrationPeriods",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentJoins_PeriodId",
                table: "StudentJoins",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentJoins_StudentId",
                table: "StudentJoins",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationPeriods_Facultys_FacultyId",
                table: "RegistrationPeriods",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationPeriods_Facultys_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropTable(
                name: "StudentJoins");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationPeriods_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "RegistrationPeriods");

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "RegistrationPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
