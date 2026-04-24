using Backend_admin.Data;
using Backend_admin.Models;
using Backend_admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_admin.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        public AdminController(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

      
        // STUDENTS
       

        // GET: api/admin/students
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var learners = await _context.Learners
                .Include(l => l.DbBookings)
                .ToListAsync();

            return Ok(new ApiResponse<List<Learner>>
            {
                Success = true,
                Message = "Students retrieved successfully",
                Data = learners
            });
        }

        // GET: api/admin/students/inactive
        [HttpGet("students/inactive")]
        public async Task<IActionResult> GetInactiveStudents()
        {
            var learners = await _context.Learners
                .Where(l => l.LearnerStatus != "Active")
                .Include(l => l.DbBookings)
                .ToListAsync();

            return Ok(new ApiResponse<List<Learner>>
            {
                Success = true,
                Message = "Inactive students retrieved successfully",
                Data = learners
            });
        }

        // GET: api/admin/student/{id}
        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var learner = await _context.Learners
                .Include(l => l.DbBookings)
                .FirstOrDefaultAsync(l => l.LearnerId == id);

            if (learner == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Learner not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Learner>
            {
                Success = true,
                Message = "Learner retrieved successfully",
                Data = learner
            });
        }

        // GET: api/admin/student/{id}/activity
        [HttpGet("student/{id}/activity")]
        public async Task<IActionResult> GetStudentActivity(int id)
        {
            var learner = await _context.Learners
                .Include(l => l.DbBookings)
                .FirstOrDefaultAsync(l => l.LearnerId == id);

            if (learner == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Learner not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Learner>
            {
                Success = true,
                Message = "Student activity retrieved successfully",
                Data = learner
            });
        }

        // DELETE: api/admin/student/{id}
        [HttpDelete("student/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var learner = await _context.Learners.FindAsync(id);

            if (learner == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Learner not found",
                    Data = null
                });
            }

            _context.Learners.Remove(learner);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Learner removed successfully",
                Data = null
            });
        }

       
        // INSTRUCTORS
        

        // GET: api/admin/instructors
        [HttpGet("instructors")]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await _context.Instructors
                .Include(i => i.DbBookings)
                .Include(i => i.DbAvailabilities)
                .ToListAsync();

            return Ok(new ApiResponse<List<Instructor>>
            {
                Success = true,
                Message = "Instructors retrieved successfully",
                Data = instructors
            });
        }

        // GET: api/admin/instructor/{id}
        [HttpGet("instructor/{id}")]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.DbBookings)
                .Include(i => i.DbAvailabilities)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Instructor not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Instructor>
            {
                Success = true,
                Message = "Instructor retrieved successfully",
                Data = instructor
            });
        }

        // DELETE: api/admin/instructor/{id}
        [HttpDelete("instructor/{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Instructor not found",
                    Data = null
                });
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Instructor removed successfully",
                Data = null
            });
        }

        
        // SYSTEM ACTIVITY
        

        // GET: api/admin/system-activity
        [HttpGet("system-activity")]
        public async Task<IActionResult> GetSystemActivity()
        {
            var learners = await _context.Learners
                .Include(l => l.DbBookings)
                .ToListAsync();

            var instructors = await _context.Instructors
                .Include(i => i.DbBookings)
                .ToListAsync();

            var bookings = await _context.Bookings.ToListAsync();

            var result = new
            {
                TotalStudents = learners.Count,
                TotalInstructors = instructors.Count,
                TotalBookings = bookings.Count,
                RecentBookings = bookings
                    .OrderByDescending(b => b.LessonDate)
                    .Take(10)
                    .ToList()
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "System activity retrieved successfully",
                Data = result
            });
        }

        
        // ADMIN SETTINGS
     

        // GET: api/admin/settings
        [HttpGet("settings")]
        public async Task<IActionResult> GetAdminSettings()
        {
            var admin = await _context.Admins.FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Admin not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Admin>
            {
                Success = true,
                Message = "Admin settings retrieved successfully",
                Data = admin
            });
        }

        // PUT: api/admin/settings/update
        [HttpPut("settings/update")]
        public async Task<IActionResult> UpdateAdminSettings([FromBody] Admin request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Admin not found",
                    Data = null
                });
            }

            admin.AdminName = request.AdminName;
            admin.AdminEmail = request.AdminEmail;
            admin.AdminPhone = request.AdminPhone;
            admin.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Admin settings updated successfully",
                Data = null
            });
        }

        // PUT: api/admin/settings/change-password
        [HttpPut("settings/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Admin not found",
                    Data = null
                });
            }

            if (!_passwordService.VerifyPassword(admin.AdminPasswordHash, request.CurrentPassword))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Current password is incorrect",
                    Data = null
                });
            }

            admin.AdminPasswordHash = _passwordService.HashPassword(request.NewPassword);
            admin.LastPasswordChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Password changed successfully",
                Data = null
            });
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}


