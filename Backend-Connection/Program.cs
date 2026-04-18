using Backend_Connection.Data;
using Backend_Connection.Models;
using Backend_Connection.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// this registers the jwt service used to generate tokens during login
builder.Services.AddSingleton<JwtService>();

// this registers the password service used for hashing and verifying passwords
builder.Services.AddSingleton<PasswordService>();

// this enables controller endpoints
builder.Services.AddControllers();

// this reads the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// this uses sql server when a connection string is provided
// if the connection string is blank, it falls back to an in memory database
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DrivingLessonDb"));
}

// this enables openapi in development
builder.Services.AddOpenApi();

// this reads jwt settings from appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// this configures jwt authentication for protected endpoints
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

// this enables authorize attributes and role checks
builder.Services.AddAuthorization();

var app = builder.Build();

// this enables openapi in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// this creates and seeds the in memory database when sql is not being used
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        db.Database.EnsureCreated();
        SeedInMemoryDatabase(db);
    }
}

// this redirects http to https
app.UseHttpsRedirection();

// this validates incoming jwt tokens
app.UseAuthentication();

// this enforces authorize rules
app.UseAuthorization();

// this maps controller routes
app.MapControllers();

app.Run();

// this adds sample data only when using the in memory database
void SeedInMemoryDatabase(ApplicationDbContext db)
{
    // this prevents duplicate seed data on restart
    if (db.Instructors.Any() || db.Learners.Any() || db.Availabilities.Any() || db.Bookings.Any())
    {
        return;
    }

    // this seeds instructors
    db.Instructors.AddRange(
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

    // this seeds learners
    db.Learners.AddRange(
        new Learner
        {
            LearnerId = 1,
            LearnerName = "Adam Lee",
            LearnerLicenceId = "L1234567",
            LearnerEmail = "adam.lee@example.com",
            LearnerPhone = "07111111111",
            LearnerPasswordHash = "",
            LearnerStatus = "Active",
            PastLessonCount = 0,
            NextLessonCount = 1,
            LearnerLessonType = "Manual",
            InstructorId = 1
        },
        new Learner
        {
            LearnerId = 2,
            LearnerName = "Maria Khan",
            LearnerLicenceId = "L7654321",
            LearnerEmail = "maria.khan@example.com",
            LearnerPhone = "07222222222",
            LearnerPasswordHash = "",
            LearnerStatus = "Active",
            PastLessonCount = 2,
            NextLessonCount = 0,
            LearnerLessonType = "Automatic",
            InstructorId = 2
        }
    );

    // this seeds availabilities
    db.Availabilities.AddRange(
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

    // this seeds bookings
    db.Bookings.Add(
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

    // this saves the seeded data
    db.SaveChanges();
}