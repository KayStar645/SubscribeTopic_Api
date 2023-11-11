using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class create_table_feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommenterId = table.Column<int>(type: "int", nullable: true),
                    ThesisId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Teachers_CommenterId",
                        column: x => x.CommenterId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Thesiss_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Thesiss",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CommenterId",
                table: "Feedbacks",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ThesisId",
                table: "Feedbacks",
                column: "ThesisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");
        }
    }
}
