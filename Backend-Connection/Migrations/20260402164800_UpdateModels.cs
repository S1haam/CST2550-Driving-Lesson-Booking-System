using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Connection.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RemovalReason",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptanceMessage",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1,
                columns: new[] { "InstructorEmail", "InstructorName" },
                values: new object[] { "abraham.smith@example.com", "Abraham Smith" });

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2,
                columns: new[] { "InstructorEmail", "InstructorName" },
                values: new object[] { "ali.john@example.com", "Ali John" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RemovalReason",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AcceptanceMessage",
                table: "StudentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1,
                columns: new[] { "InstructorEmail", "InstructorName" },
                values: new object[] { "john.smith@example.com", "John Smith" });

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2,
                columns: new[] { "InstructorEmail", "InstructorName" },
                values: new object[] { "sarah.johnson@example.com", "Sarah Johnson" });
        }
    }
}
