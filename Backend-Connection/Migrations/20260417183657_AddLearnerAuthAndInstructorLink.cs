using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Connection.Migrations
{
    /// <inheritdoc />
    public partial class AddLearnerAuthAndInstructorLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "Learners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LearnerLessonType",
                table: "Learners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorPasswordHash",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1,
                column: "InstructorPasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2,
                column: "InstructorPasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 1,
                columns: new[] { "InstructorId", "LearnerLessonType", "LearnerPasswordHash" },
                values: new object[] { 1, "Manual", "" });

            migrationBuilder.UpdateData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 2,
                columns: new[] { "InstructorId", "LearnerLessonType", "LearnerPasswordHash" },
                values: new object[] { 2, "Automatic", "" });

            migrationBuilder.CreateIndex(
                name: "IX_Learners_InstructorId",
                table: "Learners",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Learners_Instructors_InstructorId",
                table: "Learners",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Learners_Instructors_InstructorId",
                table: "Learners");

            migrationBuilder.DropIndex(
                name: "IX_Learners_InstructorId",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "LearnerLessonType",
                table: "Learners");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorPasswordHash",
                table: "Instructors",
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
                column: "InstructorPasswordHash",
                value: "hash1");

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2,
                column: "InstructorPasswordHash",
                value: "hash2");

            migrationBuilder.UpdateData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 1,
                column: "LearnerPasswordHash",
                value: "hash3");

            migrationBuilder.UpdateData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 2,
                column: "LearnerPasswordHash",
                value: "hash4");
        }
    }
}
