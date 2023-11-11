using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class create_table_thesis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThesisMajors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    MajorId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisMajors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisMajors_Majors_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Majors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThesisMajors_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Thesiss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinQuantity = table.Column<int>(type: "int", nullable: true),
                    MaxQuantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LecturerThesisId = table.Column<int>(type: "int", nullable: true),
                    ProposedStudentId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thesiss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thesiss_Students_ProposedStudentId",
                        column: x => x.ProposedStudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Thesiss_Teachers_LecturerThesisId",
                        column: x => x.LecturerThesisId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Thesiss_Thesiss_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Thesiss",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThesisInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    ThesisId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisInstructions_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThesisInstructions_Thesiss_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Thesiss",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThesisReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    ThesisId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisReviews_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThesisReviews_Thesiss_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Thesiss",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThesisInstructions_TeacherId",
                table: "ThesisInstructions",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisInstructions_ThesisId",
                table: "ThesisInstructions",
                column: "ThesisId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisMajors_MajorId",
                table: "ThesisMajors",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisMajors_TeacherId",
                table: "ThesisMajors",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisReviews_TeacherId",
                table: "ThesisReviews",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisReviews_ThesisId",
                table: "ThesisReviews",
                column: "ThesisId");

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_LecturerThesisId",
                table: "Thesiss",
                column: "LecturerThesisId");

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_ParentId",
                table: "Thesiss",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_ProposedStudentId",
                table: "Thesiss",
                column: "ProposedStudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesisInstructions");

            migrationBuilder.DropTable(
                name: "ThesisMajors");

            migrationBuilder.DropTable(
                name: "ThesisReviews");

            migrationBuilder.DropTable(
                name: "Thesiss");
        }
    }
}
