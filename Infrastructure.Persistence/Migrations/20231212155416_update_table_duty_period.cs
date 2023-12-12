using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_duty_period : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "Duties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Duties_PeriodId",
                table: "Duties",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Duties_RegistrationPeriods_PeriodId",
                table: "Duties",
                column: "PeriodId",
                principalTable: "RegistrationPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Duties_RegistrationPeriods_PeriodId",
                table: "Duties");

            migrationBuilder.DropIndex(
                name: "IX_Duties_PeriodId",
                table: "Duties");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Duties");
        }
    }
}
