using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_table_student_join_new1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentJoins_RegistrationPeriods_PeriodId",
                table: "StudentJoins");

            migrationBuilder.DropIndex(
                name: "IX_StudentJoins_PeriodId",
                table: "StudentJoins");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "StudentJoins");

            migrationBuilder.CreateIndex(
                name: "IX_StudentJoins_RegistrationPeriodId",
                table: "StudentJoins",
                column: "RegistrationPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentJoins_RegistrationPeriods_RegistrationPeriodId",
                table: "StudentJoins",
                column: "RegistrationPeriodId",
                principalTable: "RegistrationPeriods",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentJoins_RegistrationPeriods_RegistrationPeriodId",
                table: "StudentJoins");

            migrationBuilder.DropIndex(
                name: "IX_StudentJoins_RegistrationPeriodId",
                table: "StudentJoins");

            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "StudentJoins",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentJoins_PeriodId",
                table: "StudentJoins",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentJoins_RegistrationPeriods_PeriodId",
                table: "StudentJoins",
                column: "PeriodId",
                principalTable: "RegistrationPeriods",
                principalColumn: "Id");
        }
    }
}
