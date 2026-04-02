// Gives access to all your model classes (Instructor, Learner, Booking, etc.)
using Backend_Connection.Models;

// Provides EF Core features like DbContext and DbSet
using Microsoft.EntityFrameworkCore;      

namespace Backend_Connection.Data
{
    // Class is an overall representation of your entire SQL Server database.

    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       // setting tables in the SQL server
       
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Learner> Learners { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<StudentRequest> StudentRequests { get; set; }
        public DbSet<LessonOutcome> LessonOutcomes { get; set; }

        // Method lets you configure relationships, constraints, and also use sample data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           //Relationship configuration

            // Instructor → Availability
            // (1-to-many)
            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.DbAvailabilities)
                .WithOne(a => a.Instructor)
                .HasForeignKey(a => a.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);   // Prevents cascading deletes

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

            // Instructor sample data
            // Sample data used for something to be present in the SQL server

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    InstructorId = 1,
                    InstructorCode = "INS001",
                    InstructorName = "Abraham Smith",
                    InstructorEmail = "abraham.smith@example.com",
                    InstructorPhone = "07123456789",
                    InstructorPasswordHash = "hash1",
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
                    InstructorPasswordHash = "hash2",
                    InstructorCarType = "Automatic",
                    InstructorStatus = "Active"
                }
            );

            // Learner sample data
            modelBuilder.Entity<Learner>().HasData(
                new Learner
                {
                    LearnerId = 1,
                    LearnerName = "Adam Lee",
                    LearnerLicenceId = "L1234567",
                    LearnerEmail = "adam.lee@example.com",
                    LearnerPhone = "07111111111",
                    LearnerPasswordHash = "hash3",
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
                    LearnerPasswordHash = "hash4",
                    LearnerStatus = "Active",
                    PastLessonCount = 2,
                    NextLessonCount = 0
                }
            );

            // Availability sample data
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

            // Bookings sample data
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