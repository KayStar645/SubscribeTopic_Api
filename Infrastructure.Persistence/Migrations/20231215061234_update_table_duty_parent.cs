using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_duty_parent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DutyId",
                table: "Duties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Duties_DutyId",
                table: "Duties",
                column: "DutyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Duties_Duties_DutyId",
                table: "Duties",
                column: "DutyId",
                principalTable: "Duties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Duties_Duties_DutyId",
                table: "Duties");

            migrationBuilder.DropIndex(
                name: "IX_Duties_DutyId",
                table: "Duties");

            migrationBuilder.DropColumn(
                name: "DutyId",
                table: "Duties");
        }
    }
}
