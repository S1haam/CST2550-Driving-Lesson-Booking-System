using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend_Connection.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "InstructorId", "InstructorCarType", "InstructorCode", "InstructorEmail", "InstructorName", "InstructorPasswordHash", "InstructorPhone", "InstructorStatus" },
                values: new object[,]
                {
                    { 1, "Manual", "INS001", "john.smith@example.com", "John Smith", "hash1", "07123456789", "Active" },
                    { 2, "Automatic", "INS002", "sarah.johnson@example.com", "Sarah Johnson", "hash2", "07987654321", "Active" }
                });

            migrationBuilder.InsertData(
                table: "Learners",
                columns: new[] { "LearnerId", "LearnerEmail", "LearnerLicenceId", "LearnerName", "LearnerPasswordHash", "LearnerPhone", "LearnerStatus", "NextLessonCount", "PastLessonCount" },
                values: new object[,]
                {
                    { 1, "adam.lee@example.com", "L1234567", "Adam Lee", "hash3", "07111111111", "Active", 1, 0 },
                    { 2, "maria.khan@example.com", "L7654321", "Maria Khan", "hash4", "07222222222", "Active", 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Availabilities",
                columns: new[] { "AvailabilityId", "AvailableDateTime", "InstructorId", "IsTaken" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, false },
                    { 2, new DateTime(2026, 4, 4, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, false },
                    { 3, new DateTime(2026, 4, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, false }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingStatus", "CreatedAt", "InstructorId", "LearnerId", "LessonDate", "LessonTime", "LessonType", "UpdatedAt" },
                values: new object[] { 1, "Confirmed", new DateTime(2026, 4, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 0, 0, 0), "Beginners", new DateTime(2026, 4, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Availabilities",
                keyColumn: "AvailabilityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Availabilities",
                keyColumn: "AvailabilityId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Availabilities",
                keyColumn: "AvailabilityId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Learners",
                keyColumn: "LearnerId",
                keyValue: 1);
        }
    }
}
