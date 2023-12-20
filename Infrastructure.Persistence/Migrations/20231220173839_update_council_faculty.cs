using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_council_faculty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "Councils",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Councils_FacultyId",
                table: "Councils",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Councils_Faculties_FacultyId",
                table: "Councils",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Councils_Faculties_FacultyId",
                table: "Councils");

            migrationBuilder.DropIndex(
                name: "IX_Councils_FacultyId",
                table: "Councils");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Councils");
        }
    }
}
