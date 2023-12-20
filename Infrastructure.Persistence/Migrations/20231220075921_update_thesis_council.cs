using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_thesis_council : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouncilId",
                table: "Thesiss",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_CouncilId",
                table: "Thesiss",
                column: "CouncilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thesiss_Councils_CouncilId",
                table: "Thesiss",
                column: "CouncilId",
                principalTable: "Councils",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thesiss_Councils_CouncilId",
                table: "Thesiss");

            migrationBuilder.DropIndex(
                name: "IX_Thesiss_CouncilId",
                table: "Thesiss");

            migrationBuilder.DropColumn(
                name: "CouncilId",
                table: "Thesiss");
        }
    }
}
