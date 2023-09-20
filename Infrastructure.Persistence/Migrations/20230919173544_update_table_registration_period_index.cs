using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_registration_period_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "RegistrationPeriods");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStart",
                table: "RegistrationPeriods",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeEnd",
                table: "RegistrationPeriods",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "RegistrationPeriods",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolYear",
                table: "RegistrationPeriods",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYear_Semester_FacultyId",
                table: "RegistrationPeriods",
                columns: new[] { "SchoolYear", "Semester", "FacultyId" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYear_Semester_Phase_FacultyId",
                table: "RegistrationPeriods",
                columns: new[] { "SchoolYear", "Semester", "Phase", "FacultyId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolYear_Semester_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_SchoolYear_Semester_Phase_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropColumn(
                name: "SchoolYear",
                table: "RegistrationPeriods");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStart",
                table: "RegistrationPeriods",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeEnd",
                table: "RegistrationPeriods",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "RegistrationPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "RegistrationPeriods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
