using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Connection.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdminPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SystemUserCount = table.Column<int>(type: "int", nullable: false),
                    SystemBookingCount = table.Column<int>(type: "int", nullable: false),
                    SystemRequestCount = table.Column<int>(type: "int", nullable: false),
                    RemovedUserId = table.Column<int>(type: "int", nullable: true),
                    RemovedUserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemovedByAdminId = table.Column<int>(type: "int", nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstructorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorCarType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.InstructorId);
                });

            migrationBuilder.CreateTable(
                name: "Learners",
                columns: table => new
                {
                    LearnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LearnerLicenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PastLessonCount = table.Column<int>(type: "int", nullable: false),
                    NextLessonCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.LearnerId);
                });

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTaken = table.Column<bool>(type: "bit", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_Availabilities_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearnerId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    LessonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LessonTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LessonType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "LearnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearnerId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    AvailabilityId = table.Column<int>(type: "int", nullable: false),
                    RequestMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcceptanceMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_StudentRequests_Availabilities_AvailabilityId",
                        column: x => x.AvailabilityId,
                        principalTable: "Availabilities",
                        principalColumn: "AvailabilityId");
                    table.ForeignKey(
                        name: "FK_StudentRequests_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId");
                    table.ForeignKey(
                        name: "FK_StudentRequests_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "LearnerId");
                });

            migrationBuilder.CreateTable(
                name: "LessonOutcomes",
                columns: table => new
                {
                    OutcomeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    LessonNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LessonOutcomeStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LessonRating = table.Column<int>(type: "int", nullable: true),
                    NotesCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonOutcomes", x => x.OutcomeId);
                    table.ForeignKey(
                        name: "FK_LessonOutcomes_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonOutcomes_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_InstructorId",
                table: "Availabilities",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_InstructorId",
                table: "Bookings",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LearnerId",
                table: "Bookings",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonOutcomes_BookingId",
                table: "LessonOutcomes",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonOutcomes_InstructorId",
                table: "LessonOutcomes",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_AvailabilityId",
                table: "StudentRequests",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_InstructorId",
                table: "StudentRequests",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_LearnerId",
                table: "StudentRequests",
                column: "LearnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "LessonOutcomes");

            migrationBuilder.DropTable(
                name: "StudentRequests");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropTable(
                name: "Learners");

            migrationBuilder.DropTable(
                name: "Instructors");
        }
    }
}
