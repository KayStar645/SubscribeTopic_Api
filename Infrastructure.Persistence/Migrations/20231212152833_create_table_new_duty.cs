using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class create_table_new_duty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentDuties");

            migrationBuilder.DropTable(
                name: "FacultyDuties");

            migrationBuilder.AddColumn<int>(
                name: "DutyId",
                table: "Thesiss",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Duties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfThesis = table.Column<int>(type: "int", nullable: true),
                    TimeEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duties_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Duties_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Duties_Teachers_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Thesiss_DutyId",
                table: "Thesiss",
                column: "DutyId");

            migrationBuilder.CreateIndex(
                name: "IX_Duties_DepartmentId",
                table: "Duties",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Duties_FacultyId",
                table: "Duties",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thesiss_Duties_DutyId",
                table: "Thesiss",
                column: "DutyId",
                principalTable: "Duties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thesiss_Duties_DutyId",
                table: "Thesiss");

            migrationBuilder.DropTable(
                name: "Duties");

            migrationBuilder.DropIndex(
                name: "IX_Thesiss_DutyId",
                table: "Thesiss");

            migrationBuilder.DropColumn(
                name: "DutyId",
                table: "Thesiss");

            migrationBuilder.CreateTable(
                name: "DepartmentDuties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfThesis = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    TimeEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeStart = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentDuties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentDuties_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentDuties_Teachers_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FacultyDuties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    FacultyId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfThesis = table.Column<int>(type: "int", nullable: true),
                    TimeEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeStart = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyDuties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyDuties_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FacultyDuties_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentDuties_DepartmentId",
                table: "DepartmentDuties",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDuties_DepartmentId",
                table: "FacultyDuties",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDuties_FacultyId",
                table: "FacultyDuties",
                column: "FacultyId");
        }
    }
}
