using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class create_table_point : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Point",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scores = table.Column<double>(type: "float", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentJoinId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Point_StudentJoins_StudentJoinId",
                        column: x => x.StudentJoinId,
                        principalTable: "StudentJoins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Point_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Point_StudentJoinId",
                table: "Point",
                column: "StudentJoinId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_TeacherId",
                table: "Point",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Point");
        }
    }
}
