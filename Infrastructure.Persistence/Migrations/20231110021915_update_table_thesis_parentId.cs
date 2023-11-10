using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_thesis_parentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thesiss_Thesiss_ParentId",
                table: "Thesiss");

            migrationBuilder.DropIndex(
                name: "IX_Thesiss_ParentId",
                table: "Thesiss");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Thesiss");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Thesiss",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_ParentId",
                table: "Thesiss",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thesiss_Thesiss_ParentId",
                table: "Thesiss",
                column: "ParentId",
                principalTable: "Thesiss",
                principalColumn: "Id");
        }
    }
}
