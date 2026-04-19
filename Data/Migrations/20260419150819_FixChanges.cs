using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Quick_Gen.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CatalogLevel", "Difficulty", "DurationWeeks", "IsFree", "IsLocked", "LessonCount", "ShortDescription", "Status", "ThumbnailUrl", "Title" },
                values: new object[,]
                {
                    { 8, 1, 0, 4, true, false, 10, "Learn fundamentals of C#", 1, null, "C# Basics" },
                    { 9, 2, 1, 6, false, false, 15, "Build REST APIs using ASP.NET Core", 1, null, "ASP.NET Core API" },
                    { 10, 3, 2, 8, false, true, 20, "Clean Architecture & DDD", 0, null, "Advanced .NET Architecture" }
                });
        }
    }
}
