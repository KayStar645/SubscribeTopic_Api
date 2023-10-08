using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class create_table_invitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Facultys_FacultyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_FacultyDuties_Facultys_FacultyId",
                table: "FacultyDuties");

            migrationBuilder.DropForeignKey(
                name: "FK_Facultys_Teachers_Dean_TeacherId",
                table: "Facultys");

            migrationBuilder.DropForeignKey(
                name: "FK_Industries_Facultys_FacultyId",
                table: "Industries");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Facultys_FacultyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationPeriods_Facultys_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facultys",
                table: "Facultys");

            migrationBuilder.RenameTable(
                name: "Facultys",
                newName: "Faculties");

            migrationBuilder.RenameIndex(
                name: "IX_Facultys_Dean_TeacherId",
                table: "Faculties",
                newName: "IX_Faculties_Dean_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeSent = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    StudentJoinId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invitations_StudentJoins_StudentJoinId",
                        column: x => x.StudentJoinId,
                        principalTable: "StudentJoins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_GroupId",
                table: "Invitations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_StudentJoinId",
                table: "Invitations",
                column: "StudentJoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                table: "Departments",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Faculties_Teachers_Dean_TeacherId",
                table: "Faculties",
                column: "Dean_TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyDuties_Faculties_FacultyId",
                table: "FacultyDuties",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Industries_Faculties_FacultyId",
                table: "Industries",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Faculties_FacultyId",
                table: "Notifications",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationPeriods_Faculties_FacultyId",
                table: "RegistrationPeriods",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Faculties_Teachers_Dean_TeacherId",
                table: "Faculties");

            migrationBuilder.DropForeignKey(
                name: "FK_FacultyDuties_Faculties_FacultyId",
                table: "FacultyDuties");

            migrationBuilder.DropForeignKey(
                name: "FK_Industries_Faculties_FacultyId",
                table: "Industries");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Faculties_FacultyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationPeriods_Faculties_FacultyId",
                table: "RegistrationPeriods");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties");

            migrationBuilder.RenameTable(
                name: "Faculties",
                newName: "Facultys");

            migrationBuilder.RenameIndex(
                name: "IX_Faculties_Dean_TeacherId",
                table: "Facultys",
                newName: "IX_Facultys_Dean_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facultys",
                table: "Facultys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Facultys_FacultyId",
                table: "Departments",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyDuties_Facultys_FacultyId",
                table: "FacultyDuties",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facultys_Teachers_Dean_TeacherId",
                table: "Facultys",
                column: "Dean_TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Industries_Facultys_FacultyId",
                table: "Industries",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Facultys_FacultyId",
                table: "Notifications",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationPeriods_Facultys_FacultyId",
                table: "RegistrationPeriods",
                column: "FacultyId",
                principalTable: "Facultys",
                principalColumn: "Id");
        }
    }
}
