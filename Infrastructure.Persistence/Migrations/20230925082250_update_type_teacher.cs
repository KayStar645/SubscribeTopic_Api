using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_type_teacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Industry_Facultys_FacultyId",
                table: "Industry");

            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Industry_IndustryId",
                table: "Majors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Industry",
                table: "Industry");

            migrationBuilder.RenameTable(
                name: "Industry",
                newName: "Industries");

            migrationBuilder.RenameIndex(
                name: "IX_Industry_FacultyId",
                table: "Industries",
                newName: "IX_Industries_FacultyId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "L",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Industries",
                table: "Industries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Industries_Facultys_FacultyId",
                table: "Industries",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Industries_IndustryId",
                table: "Majors",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Industries_Facultys_FacultyId",
                table: "Industries");

            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Industries_IndustryId",
                table: "Majors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Industries",
                table: "Industries");

            migrationBuilder.RenameTable(
                name: "Industries",
                newName: "Industry");

            migrationBuilder.RenameIndex(
                name: "IX_Industries_FacultyId",
                table: "Industry",
                newName: "IX_Industry_FacultyId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "L");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Industry",
                table: "Industry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Industry_Facultys_FacultyId",
                table: "Industry",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Industry_IndustryId",
                table: "Majors",
                column: "IndustryId",
                principalTable: "Industry",
                principalColumn: "Id");
        }
    }
}
