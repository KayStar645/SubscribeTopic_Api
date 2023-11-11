using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_table_feedback_base : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Feedbacks",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Feedbacks",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Feedbacks",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Feedbacks");
        }
    }
}
