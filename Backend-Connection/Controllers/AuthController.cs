using Backend_Connection.Data;
using Backend_Connection.Models;
using Backend_Connection.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;

        // this injects the database context and auth services into the controller
        public AuthController(
            ApplicationDbContext context,
            JwtService jwtService,
            PasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        // this logs a user in by checking their email password and role
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

            var email = request.Email?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";
            var role = request.Role?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "email, password and role are required",
                    Data = null
                });
            }

            if (!new EmailAddressAttribute().IsValid(email))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid email format",
                    Data = null
                });
            }

            if (role != "learner" && role != "instructor" && role != "admin")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid role",
                    Data = null
                });
            }

            if (role == "learner")
            {
                var learner = await _context.Learners
                    .FirstOrDefaultAsync(x => x.LearnerEmail.ToLower() == email.ToLower());

                if (learner == null || string.IsNullOrWhiteSpace(learner.LearnerPasswordHash))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "invalid learner login",
                        Data = null
                    });
                }

                var valid = _passwordService.VerifyPassword(
                    learner.LearnerPasswordHash,
                    password
                );

                if (!valid)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "invalid learner login",
                        Data = null
                    });
                }

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

            if (role == "instructor")
            {
                var instructor = await _context.Instructors
                    .FirstOrDefaultAsync(x => x.InstructorEmail.ToLower() == email.ToLower());

                if (instructor == null || string.IsNullOrWhiteSpace(instructor.InstructorPasswordHash))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "invalid instructor login",
                        Data = null
                    });
                }

                var valid = _passwordService.VerifyPassword(
                    instructor.InstructorPasswordHash,
                    password
                );

                if (!valid)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "invalid instructor login",
                        Data = null
                    });
                }

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

            var admin = await _context.Admins
                .FirstOrDefaultAsync(x => x.AdminEmail.ToLower() == email.ToLower());

            if (admin == null || string.IsNullOrWhiteSpace(admin.AdminPasswordHash))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid admin login",
                    Data = null
                });
            }

            var adminValid = _passwordService.VerifyPassword(
                admin.AdminPasswordHash,
                password
            );

            if (!adminValid)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid admin login",
                    Data = null
                });
            }

            var adminToken = _jwtService.GenerateToken(
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
                    Token = adminToken,
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
            {
                return BadRequest(ResponseError("request is required"));
            }

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

            if (!new EmailAddressAttribute().IsValid(learnerEmail))
            {
                return BadRequest(ResponseError("invalid email format"));
            }

            if (password.Length < 6)
            {
                return BadRequest(ResponseError("password must be at least 6 characters"));
            }

            if (learnerLessonType != "Manual" && learnerLessonType != "Automatic")
            {
                return BadRequest(ResponseError("lesson type must be Manual or Automatic"));
            }

            if (await _context.Learners.AnyAsync(x => x.LearnerEmail.ToLower() == learnerEmail.ToLower()))
            {
                return BadRequest(ResponseError("email already exists"));
            }

            var instructorExists = await _context.Instructors
                .AnyAsync(x => x.InstructorId == request.SelectedInstructorId);

            if (!instructorExists)
            {
                return BadRequest(ResponseError("selected instructor does not exist"));
            }

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
                InstructorId = request.SelectedInstructorId
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
            {
                return BadRequest(ResponseError("request is required"));
            }

            var instructorCode = request.InstructorCode?.Trim() ?? "";
            var instructorName = request.InstructorName?.Trim() ?? "";
            var instructorEmail = request.InstructorEmail?.Trim() ?? "";
            var instructorPhone = request.InstructorPhone?.Trim() ?? "";
            var instructorCarType = request.InstructorCarType?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(instructorCode) ||
                string.IsNullOrWhiteSpace(instructorName) ||
                string.IsNullOrWhiteSpace(instructorEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(instructorCarType))
            {
                return BadRequest(ResponseError("all instructor fields are required"));
            }

            if (!new EmailAddressAttribute().IsValid(instructorEmail))
            {
                return BadRequest(ResponseError("invalid email format"));
            }

            if (password.Length < 6)
            {
                return BadRequest(ResponseError("password must be at least 6 characters"));
            }

            if (instructorCarType != "Manual" && instructorCarType != "Automatic")
            {
                return BadRequest(ResponseError("car type must be Manual or Automatic"));
            }

            if (await _context.Instructors.AnyAsync(x => x.InstructorEmail.ToLower() == instructorEmail.ToLower()))
            {
                return BadRequest(ResponseError("email already exists"));
            }

            var instructor = new Instructor
            {
                InstructorCode = instructorCode,
                InstructorName = instructorName,
                InstructorEmail = instructorEmail,
                InstructorPhone = instructorPhone,
                InstructorCarType = instructorCarType,
                InstructorPasswordHash = _passwordService.HashPassword(password),
                InstructorStatus = "Active"
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

        // this registers an admin account after validating admin signup fields
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            if (request == null)
            {
                return BadRequest(ResponseError("request is required"));
            }

            var adminEmail = request.AdminEmail?.Trim() ?? "";
            var password = request.Password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(ResponseError("admin email and password are required"));
            }

            if (!new EmailAddressAttribute().IsValid(adminEmail))
            {
                return BadRequest(ResponseError("invalid email format"));
            }

            if (password.Length < 6)
            {
                return BadRequest(ResponseError("password must be at least 6 characters"));
            }

            if (await _context.Admins.AnyAsync(x => x.AdminEmail.ToLower() == adminEmail.ToLower()))
            {
                return BadRequest(ResponseError("email already exists"));
            }

            var admin = new Admin
            {
                AdminEmail = adminEmail,
                AdminPasswordHash = _passwordService.HashPassword(password),
                AdminRole = "Admin"
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

        // this builds a standard error response object
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

    // this stores login request data sent from the frontend
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }

    // this stores learner registration data sent from the frontend
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

    // this stores instructor registration data sent from the frontend
    public class RegisterInstructorRequest
    {
        public string InstructorCode { get; set; } = "";
        public string InstructorName { get; set; } = "";
        public string InstructorEmail { get; set; } = "";
        public string? InstructorPhone { get; set; }
        public string InstructorCarType { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // this stores admin registration data sent from the frontend
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