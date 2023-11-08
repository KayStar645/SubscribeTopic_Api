using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_thesis_major : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThesisMajors_Teachers_TeacherId",
                table: "ThesisMajors");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "ThesisMajors",
                newName: "ThesisId");

            migrationBuilder.RenameIndex(
                name: "IX_ThesisMajors_TeacherId",
                table: "ThesisMajors",
                newName: "IX_ThesisMajors_ThesisId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThesisMajors_Thesiss_ThesisId",
                table: "ThesisMajors",
                column: "ThesisId",
                principalTable: "Thesiss",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThesisMajors_Thesiss_ThesisId",
                table: "ThesisMajors");

            migrationBuilder.RenameColumn(
                name: "ThesisId",
                table: "ThesisMajors",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_ThesisMajors_ThesisId",
                table: "ThesisMajors",
                newName: "IX_ThesisMajors_TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThesisMajors_Teachers_TeacherId",
                table: "ThesisMajors",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
