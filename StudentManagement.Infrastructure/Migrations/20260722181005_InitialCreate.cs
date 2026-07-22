using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Age", "Course", "CreatedDate", "Email", "Name" },
                values: new object[,]
                {
                    { 1, 21, "Computer Science", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "aarav.sharma@example.com", "Aarav Sharma" },
                    { 2, 22, "Information Technology", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "priya.patel@example.com", "Priya Patel" },
                    { 3, 20, "Data Science", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "rohan.verma@example.com", "Rohan Verma" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "PasswordHash", "PasswordSalt", "Role", "Username" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@zestindia.com", new byte[] { 137, 186, 67, 37, 170, 43, 222, 185, 16, 63, 49, 52, 67, 143, 150, 91, 152, 231, 238, 49, 193, 61, 255, 242, 116, 83, 96, 127, 108, 61, 10, 131, 203, 103, 87, 146, 109, 131, 224, 135, 105, 123, 214, 26, 76, 115, 219, 100, 214, 157, 58, 26, 135, 97, 132, 118, 49, 179, 62, 67, 39, 101, 124, 69 }, new byte[] { 213, 80, 167, 73, 47, 230, 14, 89, 98, 195, 241, 205, 152, 74, 79, 74, 96, 113, 77, 203, 244, 113, 194, 132, 81, 2, 199, 202, 161, 183, 38, 93, 226, 106, 154, 16, 95, 243, 55, 217, 221, 185, 42, 103, 245, 41, 98, 22, 223, 73, 174, 167, 44, 97, 139, 220, 184, 208, 242, 55, 208, 225, 59, 6, 185, 71, 136, 15, 190, 14, 245, 2, 153, 71, 247, 149, 116, 144, 190, 3, 228, 219, 173, 139, 182, 172, 140, 75, 243, 114, 210, 198, 25, 72, 179, 188, 112, 93, 208, 157, 84, 199, 209, 117, 249, 51, 136, 198, 46, 223, 196, 52, 10, 83, 145, 117, 178, 244, 125, 128, 34, 147, 205, 136, 250, 240, 193, 90 }, "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
