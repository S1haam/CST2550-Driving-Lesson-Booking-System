using Backend_Connection.Data;
using Backend_Connection.Models;
using Backend_Connection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        // this gives the controller access to the database and password service
        public AdminController(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // this gets all students for the admin student database page
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Learners
                .OrderBy(x => x.LearnerId)
                .Select(x => new AdminStudentDto
                {
                    StudentId = x.LearnerId.ToString(),
                    Name = x.LearnerName,
                    Email = x.LearnerEmail,
                    Phone = x.LearnerPhone,
                    LessonsBooked = x.PastLessonCount + x.NextLessonCount
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminStudentDto>>
            {
                Success = true,
                Message = "students retrieved successfully",
                Data = students
            });
        }

        // this gets one student profile for the admin student profile page
        [HttpGet("student/{id:int}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var learner = await _context.Learners
                .Include(x => x.Instructor)
                .Include(x => x.DbBookings)
                .FirstOrDefaultAsync(x => x.LearnerId == id);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "student not found",
                    Data = null
                });
            }

            var lastLesson = learner.DbBookings
                .Where(x => x.LessonDate.Add(x.LessonTime) < DateTime.Now)
                .OrderByDescending(x => x.LessonDate)
                .ThenByDescending(x => x.LessonTime)
                .FirstOrDefault();

            var nextLesson = learner.DbBookings
                .Where(x => x.LessonDate.Add(x.LessonTime) >= DateTime.Now)
                .OrderBy(x => x.LessonDate)
                .ThenBy(x => x.LessonTime)
                .FirstOrDefault();

            var student = new AdminStudentProfileDto
            {
                Name = learner.LearnerName,
                Email = learner.LearnerEmail,
                Phone = learner.LearnerPhone,
                Status = learner.LearnerStatus,
                LessonsBooked = learner.PastLessonCount + learner.NextLessonCount,
                LastLesson = lastLesson == null ? "N/A" : lastLesson.LessonDate.ToString("dd/MM/yyyy"),
                NextLesson = nextLesson == null ? "N/A" : nextLesson.LessonDate.ToString("dd/MM/yyyy"),
                InstructorName = learner.Instructor == null ? "N/A" : learner.Instructor.InstructorName,
                JoinedOn = "N/A",
                LastLogin = "N/A"
            };

            return Ok(new ApiResponse<AdminStudentProfileDto>
            {
                Success = true,
                Message = "student retrieved successfully",
                Data = student
            });
        }

        // this gets inactive students for the inactive student removal page
        [HttpGet("students/inactive")]
        public async Task<IActionResult> GetInactiveStudents()
        {
            var students = await _context.Learners
                .Where(x => x.LearnerStatus != "Active")
                .OrderBy(x => x.LearnerId)
                .Select(x => new AdminInactiveStudentDto
                {
                    StudentId = x.LearnerId.ToString(),
                    Name = x.LearnerName,
                    Email = x.LearnerEmail,
                    LastActive = "N/A"
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminInactiveStudentDto>>
            {
                Success = true,
                Message = "inactive students retrieved successfully",
                Data = students
            });
        }

        // this removes a student from the database
        [HttpDelete("student/{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var learner = await _context.Learners
                .Include(x => x.DbBookings)
                .Include(x => x.StudentRequests)
                .FirstOrDefaultAsync(x => x.LearnerId == id);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "student not found",
                    Data = null
                });
            }

            if (learner.DbBookings.Any())
                _context.Bookings.RemoveRange(learner.DbBookings);

            if (learner.StudentRequests.Any())
                _context.StudentRequests.RemoveRange(learner.StudentRequests);

            _context.Learners.Remove(learner);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "student removed successfully",
                Data = null
            });
        }

        // this gets all instructors for the admin instructor database page
        [HttpGet("instructors")]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await _context.Instructors
                .Include(x => x.DbAvailabilities)
                .OrderBy(x => x.InstructorId)
                .Select(x => new AdminInstructorDto
                {
                    InstructorId = x.InstructorId.ToString(),
                    Name = x.InstructorName,
                    Email = x.InstructorEmail,
                    Phone = x.InstructorPhone,
                    Availability = x.DbAvailabilities.Any(a => !a.IsTaken) ? "Available" : "Unavailable"
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminInstructorDto>>
            {
                Success = true,
                Message = "instructors retrieved successfully",
                Data = instructors
            });
        }

        // this gets one instructor profile for the admin instructor profile page
        [HttpGet("instructor/{id:int}")]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            var instructor = await _context.Instructors
                .Include(x => x.DbBookings)
                .ThenInclude(x => x.Learner)
                .Include(x => x.DbLearners)
                .FirstOrDefaultAsync(x => x.InstructorId == id);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            var nextBooking = instructor.DbBookings
                .Where(x => x.LessonDate.Add(x.LessonTime) >= DateTime.Now)
                .OrderBy(x => x.LessonDate)
                .ThenBy(x => x.LessonTime)
                .FirstOrDefault();

            var profile = new AdminInstructorProfileDto
            {
                Name = instructor.InstructorName,
                Email = instructor.InstructorEmail,
                Phone = instructor.InstructorPhone,
                Status = instructor.InstructorStatus,
                LessonsTaught = instructor.DbBookings.Count(x => x.LessonDate.Add(x.LessonTime) < DateTime.Now),
                CurrentStudents = instructor.DbLearners.Count(x => x.LearnerStatus != "Removed"),
                NextLesson = nextBooking == null ? "N/A" : nextBooking.LessonDate.ToString("dd/MM/yyyy"),
                NextStudent = nextBooking?.Learner == null ? "N/A" : nextBooking.Learner.LearnerName,
                JoinedOn = "N/A",
                LastLogin = "N/A"
            };

            return Ok(new ApiResponse<AdminInstructorProfileDto>
            {
                Success = true,
                Message = "instructor retrieved successfully",
                Data = profile
            });
        }

        // this removes an instructor from the database
        [HttpDelete("instructor/{id:int}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(x => x.InstructorId == id);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "instructor removed successfully",
                Data = null
            });
        }

        // this gets system activity data for the admin system activity page
        [HttpGet("system-activity")]
        public async Task<IActionResult> GetSystemActivity()
        {
            var totalStudents = await _context.Learners.CountAsync();
            var totalInstructors = await _context.Instructors.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();

            var recentBookings = await _context.Bookings
                .OrderByDescending(x => x.LessonDate)
                .ThenByDescending(x => x.LessonTime)
                .Take(10)
                .Select(x => new AdminRecentBookingDto
                {
                    BookingId = x.BookingId,
                    LessonDate = x.LessonDate.ToString("dd/MM/yyyy"),
                    Status = x.BookingStatus
                })
                .ToListAsync();

            var data = new AdminSystemActivityDto
            {
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalBookings = totalBookings,
                RecentBookings = recentBookings
            };

            return Ok(new ApiResponse<AdminSystemActivityDto>
            {
                Success = true,
                Message = "system activity retrieved successfully",
                Data = data
            });
        }

        // this gets the logged in admin settings
        [HttpGet("settings")]
        public async Task<IActionResult> GetAdminSettings()
        {
            var adminId = GetLoggedInAdminId();

            if (adminId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var admin = await _context.Admins
                .Where(x => x.AdminId == adminId.Value)
                .Select(x => new AdminSettingsDto
                {
                    AdminId = x.AdminId,
                    AdminName = x.AdminName,
                    AdminEmail = x.AdminEmail,
                    AdminPhone = x.AdminPhone ?? "",
                    Status = x.Status,
                    LastPasswordChange = x.LastPasswordChange
                })
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "admin not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<AdminSettingsDto>
            {
                Success = true,
                Message = "admin settings retrieved successfully",
                Data = admin
            });
        }

        // this updates the logged in admin settings
        [HttpPut("settings/update")]
        public async Task<IActionResult> UpdateAdminSettings([FromBody] UpdateAdminSettingsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "request is required",
                    Data = null
                });
            }

            var adminId = GetLoggedInAdminId();

            if (adminId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(x => x.AdminId == adminId.Value);

            if (admin == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "admin not found",
                    Data = null
                });
            }

            admin.AdminName = request.AdminName?.Trim() ?? "";
            admin.AdminEmail = request.AdminEmail?.Trim() ?? "";
            admin.AdminPhone = request.AdminPhone?.Trim() ?? "";
            admin.Status = request.Status?.Trim() ?? "Active";

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "admin settings updated successfully",
                Data = null
            });
        }

        // this changes the logged in admin password
        [HttpPut("settings/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] AdminChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "request is required",
                    Data = null
                });
            }

            var adminId = GetLoggedInAdminId();

            if (adminId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(x => x.AdminId == adminId.Value);

            if (admin == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "admin not found",
                    Data = null
                });
            }

            if (!_passwordService.VerifyPassword(admin.AdminPasswordHash, request.CurrentPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "current password is incorrect",
                    Data = null
                });
            }

            admin.AdminPasswordHash = _passwordService.HashPassword(request.NewPassword);
            admin.LastPasswordChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "password changed successfully",
                Data = null
            });
        }

        // this reads the admin id from the jwt token
        private int? GetLoggedInAdminId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int adminId))
                return null;

            return adminId;
        }
    }

    // this matches adminstudents.razor
    public class AdminStudentDto
    {
        public string StudentId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public int LessonsBooked { get; set; }
    }

    // this matches adminstudentprofile.razor
    public class AdminStudentProfileDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Status { get; set; } = "";
        public int LessonsBooked { get; set; }
        public string LastLesson { get; set; } = "";
        public string NextLesson { get; set; } = "";
        public string InstructorName { get; set; } = "";
        public string JoinedOn { get; set; } = "";
        public string LastLogin { get; set; } = "";
    }

    // this matches admininstructors.razor
    public class AdminInstructorDto
    {
        public string InstructorId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Availability { get; set; } = "";
    }

    // this matches admininstructorprofile.razor
    public class AdminInstructorProfileDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Status { get; set; } = "";
        public int LessonsTaught { get; set; }
        public int CurrentStudents { get; set; }
        public string NextLesson { get; set; } = "";
        public string NextStudent { get; set; } = "";
        public string JoinedOn { get; set; } = "";
        public string LastLogin { get; set; } = "";
    }

    // this matches inactive student rows
    public class AdminInactiveStudentDto
    {
        public string StudentId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string LastActive { get; set; } = "";
    }

    // this matches admin settings data
    public class AdminSettingsDto
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public string AdminPhone { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime LastPasswordChange { get; set; }
    }

    // this stores admin settings update data
    public class UpdateAdminSettingsRequest
    {
        public string AdminName { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public string AdminPhone { get; set; } = "";
        public string Status { get; set; } = "";
    }

    // this stores admin password change data
    public class AdminChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }

    // this matches the system activity page
    public class AdminSystemActivityDto
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalBookings { get; set; }
        public List<AdminRecentBookingDto> RecentBookings { get; set; } = new();
    }

    // this matches recent booking rows
    public class AdminRecentBookingDto
    {
        public int BookingId { get; set; }
        public string LessonDate { get; set; } = "";
        public string Status { get; set; } = "";
    }
}