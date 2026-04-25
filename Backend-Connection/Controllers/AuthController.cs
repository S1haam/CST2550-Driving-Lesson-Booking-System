using Backend_Connection.Data;
using Backend_Connection.Models;
using Backend_Connection.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;

        // this gives the controller access to the database, jwt service and password service
        public AuthController(
            ApplicationDbContext context,
            JwtService jwtService,
            PasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        // this logs in a learner, instructor or admin based on the role sent from the frontend
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(ResponseError("request is required"));

            var email = request.Email?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";
            var role = request.Role?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(ResponseError("email, password and role are required"));
            }

            if (!IsValidEmail(email))
                return BadRequest(ResponseError("invalid email format"));

            if (role != "learner" && role != "instructor" && role != "admin")
                return BadRequest(ResponseError("invalid role"));

            if (role == "learner")
                return await LoginLearner(email, password);

            if (role == "instructor")
                return await LoginInstructor(email, password);

            return await LoginAdmin(email, password);
        }

        // this checks learner login details and saves last login time
        private async Task<IActionResult> LoginLearner(string email, string password)
        {
            var cleanEmail = email.Trim().ToLower();

            var learner = await _context.Learners
                .FirstOrDefaultAsync(x => x.LearnerEmail.ToLower() == cleanEmail);

            if (learner == null || string.IsNullOrWhiteSpace(learner.LearnerPasswordHash))
                return Unauthorized(ResponseError("invalid learner login"));

            var valid = _passwordService.VerifyPassword(learner.LearnerPasswordHash, password);

            if (!valid)
                return Unauthorized(ResponseError("invalid learner login"));

            learner.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(
                learner.LearnerId,
                learner.LearnerEmail,
                "Learner"
            );

            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "learner login successful",
                Data = new AuthResponse
                {
                    Token = token,
                    Role = "Learner",
                    UserId = learner.LearnerId,
                    Email = learner.LearnerEmail
                }
            });
        }

        // this checks instructor login details and saves last login time
        private async Task<IActionResult> LoginInstructor(string email, string password)
        {
            var cleanEmail = email.Trim().ToLower();

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(x => x.InstructorEmail.ToLower() == cleanEmail);

            if (instructor == null || string.IsNullOrWhiteSpace(instructor.InstructorPasswordHash))
                return Unauthorized(ResponseError("invalid instructor login"));

            var valid = _passwordService.VerifyPassword(instructor.InstructorPasswordHash, password);

            if (!valid)
                return Unauthorized(ResponseError("invalid instructor login"));

            instructor.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(
                instructor.InstructorId,
                instructor.InstructorEmail,
                "Instructor"
            );

            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "instructor login successful",
                Data = new AuthResponse
                {
                    Token = token,
                    Role = "Instructor",
                    UserId = instructor.InstructorId,
                    Email = instructor.InstructorEmail
                }
            });
        }

        // this checks admin login details and returns an admin token if they are valid
        private async Task<IActionResult> LoginAdmin(string email, string password)
        {
            var cleanEmail = email.Trim().ToLower();

            var admin = await _context.Admins
                .FirstOrDefaultAsync(x => x.AdminEmail.ToLower() == cleanEmail);

            if (admin == null || string.IsNullOrWhiteSpace(admin.AdminPasswordHash))
                return Unauthorized(ResponseError("invalid admin login"));

            var valid = _passwordService.VerifyPassword(admin.AdminPasswordHash, password);

            if (!valid)
                return Unauthorized(ResponseError("invalid admin login"));

            var token = _jwtService.GenerateToken(
                admin.AdminId,
                admin.AdminEmail,
                "Admin"
            );

            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "admin login successful",
                Data = new AuthResponse
                {
                    Token = token,
                    Role = "Admin",
                    UserId = admin.AdminId,
                    Email = admin.AdminEmail
                }
            });
        }

        // this registers a learner account after validating all learner signup fields
        [HttpPost("register/learner")]
        public async Task<IActionResult> RegisterLearner([FromBody] RegisterLearnerRequest request)
        {
            if (request == null)
                return BadRequest(ResponseError("request is required"));

            var learnerName = request.LearnerName?.Trim() ?? "";
            var learnerLicenceId = request.LearnerLicenceId?.Trim() ?? "";
            var learnerEmail = request.LearnerEmail?.Trim() ?? "";
            var learnerPhone = request.LearnerPhone?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";
            var learnerLessonType = request.LearnerLessonType?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(learnerName) ||
                string.IsNullOrWhiteSpace(learnerLicenceId) ||
                string.IsNullOrWhiteSpace(learnerEmail) ||
                string.IsNullOrWhiteSpace(learnerPhone) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(learnerLessonType) ||
                request.SelectedInstructorId <= 0)
            {
                return BadRequest(ResponseError("all learner fields are required"));
            }

            if (!IsValidEmail(learnerEmail))
                return BadRequest(ResponseError("invalid email format"));

            if (!IsValidPassword(password))
                return BadRequest(ResponseError("password must be at least 6 characters and include one uppercase letter and one number"));

            if (!IsValidPhone(learnerPhone))
                return BadRequest(ResponseError("phone number must be exactly 11 numbers"));

            if (learnerLessonType != "Manual" && learnerLessonType != "Automatic")
                return BadRequest(ResponseError("lesson type must be Manual or Automatic"));

            if (await EmailExistsAcrossAllUsers(learnerEmail))
                return BadRequest(ResponseError("email already exists"));

            var instructorExists = await _context.Instructors
                .AnyAsync(x => x.InstructorId == request.SelectedInstructorId);

            if (!instructorExists)
                return BadRequest(ResponseError("selected instructor does not exist"));

            var learner = new Learner
            {
                LearnerName = learnerName,
                LearnerLicenceId = learnerLicenceId,
                LearnerEmail = learnerEmail,
                LearnerPhone = learnerPhone,
                LearnerPasswordHash = _passwordService.HashPassword(password),
                LearnerStatus = "Active",
                PastLessonCount = 0,
                NextLessonCount = 0,
                LearnerLessonType = learnerLessonType,
                InstructorId = request.SelectedInstructorId,
                CreatedAt = DateTime.UtcNow,
                LastLogin = null
            };

            _context.Learners.Add(learner);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "learner registered successfully",
                Data = null
            });
        }

        // this registers an instructor account after validating all instructor signup fields
        [HttpPost("register/instructor")]
        public async Task<IActionResult> RegisterInstructor([FromBody] RegisterInstructorRequest request)
        {
            if (request == null)
                return BadRequest(ResponseError("request is required"));

            var instructorCode = request.InstructorCode?.Trim() ?? "";
            var instructorName = request.InstructorName?.Trim() ?? "";
            var instructorEmail = request.InstructorEmail?.Trim() ?? "";
            var instructorPhone = request.InstructorPhone?.Trim() ?? "";
            var instructorCarType = request.InstructorCarType?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(instructorCode) ||
                string.IsNullOrWhiteSpace(instructorName) ||
                string.IsNullOrWhiteSpace(instructorEmail) ||
                string.IsNullOrWhiteSpace(instructorPhone) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(instructorCarType))
            {
                return BadRequest(ResponseError("all instructor fields are required"));
            }

            if (!IsValidEmail(instructorEmail))
                return BadRequest(ResponseError("invalid email format"));

            if (!IsValidPassword(password))
                return BadRequest(ResponseError("password must be at least 6 characters and include one uppercase letter and one number"));

            if (!IsValidPhone(instructorPhone))
                return BadRequest(ResponseError("phone number must be exactly 11 numbers"));

            if (instructorCarType != "Manual" && instructorCarType != "Automatic")
                return BadRequest(ResponseError("car type must be Manual or Automatic"));

            if (await EmailExistsAcrossAllUsers(instructorEmail))
                return BadRequest(ResponseError("email already exists"));

            var instructor = new Instructor
            {
                InstructorCode = instructorCode,
                InstructorName = instructorName,
                InstructorEmail = instructorEmail,
                InstructorPhone = instructorPhone,
                InstructorCarType = instructorCarType,
                InstructorPasswordHash = _passwordService.HashPassword(password),
                InstructorStatus = "Active",
                CreatedAt = DateTime.UtcNow,
                LastLogin = null
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "instructor registered successfully",
                Data = null
            });
        }

        // this registers an admin account and adds default admin profile values for admin settings later
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            if (request == null)
                return BadRequest(ResponseError("request is required"));

            var adminEmail = request.AdminEmail?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(ResponseError("admin email and password are required"));
            }

            if (!IsValidEmail(adminEmail))
                return BadRequest(ResponseError("invalid email format"));

            if (!IsValidPassword(password))
                return BadRequest(ResponseError("password must be at least 6 characters and include one uppercase letter and one number"));

            if (await EmailExistsAcrossAllUsers(adminEmail))
                return BadRequest(ResponseError("email already exists"));

            var admin = new Admin
            {
                AdminEmail = adminEmail,
                AdminPasswordHash = _passwordService.HashPassword(password),
                AdminRole = "Admin",
                AdminName = "Admin User",
                AdminPhone = "",
                Status = "Active",
                LastPasswordChange = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "admin registered successfully",
                Data = null
            });
        }

        // this checks if an email is already used by learner, instructor or admin
        private async Task<bool> EmailExistsAcrossAllUsers(string email)
        {
            var cleanEmail = email.Trim().ToLower();

            var learnerExists = await _context.Learners
                .AnyAsync(x => x.LearnerEmail.ToLower() == cleanEmail);

            var instructorExists = await _context.Instructors
                .AnyAsync(x => x.InstructorEmail.ToLower() == cleanEmail);

            var adminExists = await _context.Admins
                .AnyAsync(x => x.AdminEmail.ToLower() == cleanEmail);

            return learnerExists || instructorExists || adminExists;
        }

        // this validates email format more strictly
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return new EmailAddressAttribute().IsValid(email) &&
                   email.Contains("@") &&
                   email.Contains(".") &&
                   Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // this validates password strength
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 6 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit);
        }

        // this validates 11 digit phone numbers
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return Regex.IsMatch(phone, @"^\d{11}$");
        }

        // this creates the standard error response used by this controller
        private ApiResponse<object> ResponseError(string message)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null
            };
        }
    }

    // this stores login details sent from the frontend
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }

    // this stores learner signup details sent from the frontend
    public class RegisterLearnerRequest
    {
        public string LearnerName { get; set; } = "";
        public string LearnerLicenceId { get; set; } = "";
        public string LearnerEmail { get; set; } = "";
        public string LearnerPhone { get; set; } = "";
        public string Password { get; set; } = "";
        public string LearnerLessonType { get; set; } = "";
        public int SelectedInstructorId { get; set; }
    }

    // this stores instructor signup details sent from the frontend
    public class RegisterInstructorRequest
    {
        public string InstructorCode { get; set; } = "";
        public string InstructorName { get; set; } = "";
        public string InstructorEmail { get; set; } = "";
        public string InstructorPhone { get; set; } = "";
        public string InstructorCarType { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // this stores admin signup details sent from the frontend
    public class RegisterAdminRequest
    {
        public string AdminEmail { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // this is returned after a successful login
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
        public string Email { get; set; } = "";
    }
}