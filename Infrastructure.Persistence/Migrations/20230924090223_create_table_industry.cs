using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class create_table_industry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Facultys_FacultyId",
                table: "Majors");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "Majors",
                newName: "IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_Majors_FacultyId",
                table: "Majors",
                newName: "IX_Majors_IndustryId");

            migrationBuilder.CreateTable(
                name: "Industry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Industry_Facultys_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Facultys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Industry_FacultyId",
                table: "Industry",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Industry_IndustryId",
                table: "Majors",
                column: "IndustryId",
                principalTable: "Industry",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Industry_IndustryId",
                table: "Majors");

            migrationBuilder.DropTable(
                name: "Industry");

            migrationBuilder.RenameColumn(
                name: "IndustryId",
                table: "Majors",
                newName: "FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_Majors_IndustryId",
                table: "Majors",
                newName: "IX_Majors_FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Facultys_FacultyId",
                table: "Majors",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");
        }
    }
}
