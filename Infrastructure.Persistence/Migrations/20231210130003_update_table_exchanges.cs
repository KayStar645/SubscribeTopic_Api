using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_exchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Exchanges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_JobId",
                table: "Exchanges",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Jobs_JobId",
                table: "Exchanges",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Jobs_JobId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_JobId",
                table: "Exchanges");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Exchanges");
        }
    }
}
