using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_report_schedule_time : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "ReportSchedules",
                newName: "TimeStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnd",
                table: "ReportSchedules",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeEnd",
                table: "ReportSchedules");

            migrationBuilder.RenameColumn(
                name: "TimeStart",
                table: "ReportSchedules",
                newName: "DateTime");
        }
    }
}
