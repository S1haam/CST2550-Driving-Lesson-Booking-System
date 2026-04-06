using Backend_Connection.Models;          // Access to your model classes
using Microsoft.EntityFrameworkCore;      // EF Core DbContext + DbSet

namespace Backend_Connection.Data
{
    // Represents the entire SQL Server database for your backend
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables in SQL Server
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Learner> Learners { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<StudentRequest> StudentRequests { get; set; }
        public DbSet<LessonOutcome> LessonOutcomes { get; set; }

        // Configure relationships + seed sample data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Relationships

            // Instructor → Availability
            // (1-to-many)
            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.DbAvailabilities)
                .WithOne(a => a.Instructor)
                .HasForeignKey(a => a.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Learner → Booking
            // (1-to-many)
            modelBuilder.Entity<Learner>()
                .HasMany(l => l.DbBookings)
                .WithOne(b => b.Learner)
                .HasForeignKey(b => b.LearnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Instructor → Booking
            // (1-to-many)
            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.DbBookings)
                .WithOne(b => b.Instructor)
                .HasForeignKey(b => b.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentRequest → Learner
            // (many-to-one)
            modelBuilder.Entity<StudentRequest>()
                .HasOne(sr => sr.Learner)
                .WithMany(l => l.StudentRequests)
                .HasForeignKey(sr => sr.LearnerId)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentRequest → Instructor
            // (many-to-one)
            modelBuilder.Entity<StudentRequest>()
                .HasOne(sr => sr.Instructor)
                .WithMany(i => i.IncomingRequests)
                .HasForeignKey(sr => sr.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentRequest → Availability
            // (many-to-one)
            modelBuilder.Entity<StudentRequest>()
                .HasOne(sr => sr.Availability)
                .WithMany()
                .HasForeignKey(sr => sr.AvailabilityId)
                .OnDelete(DeleteBehavior.NoAction);

            //Samples data

            // Instructors
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    InstructorId = 1,
                    InstructorCode = "INS001",
                    InstructorName = "Abraham Smith",
                    InstructorEmail = "abraham.smith@example.com",
                    InstructorPhone = "07123456789",
                    InstructorCarType = "Manual",
                    InstructorStatus = "Active"
                },
                new Instructor
                {
                    InstructorId = 2,
                    InstructorCode = "INS002",
                    InstructorName = "Ali John",
                    InstructorEmail = "ali.john@example.com",
                    InstructorPhone = "07987654321",
                    InstructorCarType = "Automatic",
                    InstructorStatus = "Active"
                }
            );

            // Learners
            modelBuilder.Entity<Learner>().HasData(
                new Learner
                {
                    LearnerId = 1,
                    LearnerName = "Adam Lee",
                    LearnerLicenceId = "L1234567",
                    LearnerEmail = "adam.lee@example.com",
                    LearnerPhone = "07111111111",
                    LearnerStatus = "Active",
                    PastLessonCount = 0,
                    NextLessonCount = 1
                },
                new Learner
                {
                    LearnerId = 2,
                    LearnerName = "Maria Khan",
                    LearnerLicenceId = "L7654321",
                    LearnerEmail = "maria.khan@example.com",
                    LearnerPhone = "07222222222",
                    LearnerStatus = "Active",
                    PastLessonCount = 2,
                    NextLessonCount = 0
                }
            );

            // Availabilities
            modelBuilder.Entity<Availability>().HasData(
                new Availability
                {
                    AvailabilityId = 1,
                    InstructorId = 1,
                    AvailableDateTime = new DateTime(2026, 04, 03, 10, 00, 00),
                    IsTaken = false
                },
                new Availability
                {
                    AvailabilityId = 2,
                    InstructorId = 1,
                    AvailableDateTime = new DateTime(2026, 04, 04, 14, 00, 00),
                    IsTaken = false
                },
                new Availability
                {
                    AvailabilityId = 3,
                    InstructorId = 2,
                    AvailableDateTime = new DateTime(2026, 04, 03, 09, 00, 00),
                    IsTaken = false
                }
            );

            // Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 1,
                    LearnerId = 1,
                    InstructorId = 1,
                    LessonDate = new DateTime(2026, 04, 05),
                    LessonTime = new TimeSpan(10, 00, 00),
                    LessonType = "Beginners",
                    BookingStatus = "Confirmed",
                    CreatedAt = new DateTime(2026, 04, 01, 12, 00, 00),
                    UpdatedAt = new DateTime(2026, 04, 01, 12, 00, 00)
                }
            );
        }
    }
}