using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_student_join_new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phase",
                table: "StudentJoins");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "StudentJoins");

            migrationBuilder.DropColumn(
                name: "isLeader",
                table: "StudentJoins");

            migrationBuilder.RenameColumn(
                name: "Semester",
                table: "StudentJoins",
                newName: "RegistrationPeriodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationPeriodId",
                table: "StudentJoins",
                newName: "Semester");

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "StudentJoins",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "StudentJoins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isLeader",
                table: "StudentJoins",
                type: "bit",
                nullable: true);
        }
    }
}
