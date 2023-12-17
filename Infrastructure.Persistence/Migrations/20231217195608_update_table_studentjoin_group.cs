using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_studentjoin_group : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_LeaderId",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_StudentJoins_GroupId",
                table: "StudentJoins",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LeaderId",
                table: "Groups",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentJoins_Groups_GroupId",
                table: "StudentJoins",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentJoins_Groups_GroupId",
                table: "StudentJoins");

            migrationBuilder.DropIndex(
                name: "IX_StudentJoins_GroupId",
                table: "StudentJoins");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LeaderId",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LeaderId",
                table: "Groups",
                column: "LeaderId",
                unique: true,
                filter: "[LeaderId] IS NOT NULL");
        }
    }
}
