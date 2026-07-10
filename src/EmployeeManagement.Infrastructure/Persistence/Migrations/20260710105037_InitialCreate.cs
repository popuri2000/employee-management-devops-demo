using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Designation", "Email", "IsActive", "JoiningDate", "Name", "Salary" },
                values: new object[,]
                {
                    { 1, "Engineering", "Senior Software Engineer", "ava.thompson@example.com", true, new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Ava Thompson", 95000m },
                    { 2, "Human Resources", "HR Manager", "liam.carter@example.com", true, new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Liam Carter", 72000m },
                    { 3, "Finance", "Financial Analyst", "sophia.nguyen@example.com", true, new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Sophia Nguyen", 68000m },
                    { 4, "Engineering", "DevOps Engineer", "noah.patel@example.com", true, new DateTime(2020, 11, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Noah Patel", 88000m }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Designation", "Email", "JoiningDate", "Name", "Salary" },
                values: new object[] { 5, "Marketing", "Marketing Specialist", "isabella.garcia@example.com", new DateTime(2023, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Isabella Garcia", 60000m });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
