using Backend_Connection.Data;
using Backend_Connection.Models;
using Backend_Connection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        // this gives the controller access to the database and password service
        public LearnerController(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // this gets all learners from the database
        [HttpGet]
        public async Task<IActionResult> GetAllLearners()
        {
            var data = await _context.Learners
                .Include(x => x.Instructor)
                .Select(x => new LearnerResponseDto
                {
                    LearnerId = x.LearnerId,
                    LearnerName = x.LearnerName,
                    LearnerLicenceId = x.LearnerLicenceId,
                    LearnerEmail = x.LearnerEmail,
                    LearnerPhone = x.LearnerPhone,
                    LearnerStatus = x.LearnerStatus,
                    PastLessonCount = x.PastLessonCount,
                    NextLessonCount = x.NextLessonCount,
                    LearnerLessonType = x.LearnerLessonType,
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<LearnerResponseDto>>
            {
                Success = true,
                Message = "learners retrieved successfully",
                Data = data
            });
        }

        // this gets one learner by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLearnerById(int id)
        {
            var learner = await _context.Learners
                .Include(x => x.Instructor)
                .Where(x => x.LearnerId == id)
                .Select(x => new LearnerResponseDto
                {
                    LearnerId = x.LearnerId,
                    LearnerName = x.LearnerName,
                    LearnerLicenceId = x.LearnerLicenceId,
                    LearnerEmail = x.LearnerEmail,
                    LearnerPhone = x.LearnerPhone,
                    LearnerStatus = x.LearnerStatus,
                    PastLessonCount = x.PastLessonCount,
                    NextLessonCount = x.NextLessonCount,
                    LearnerLessonType = x.LearnerLessonType,
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : null
                })
                .FirstOrDefaultAsync();

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<LearnerResponseDto>
            {
                Success = true,
                Message = "learner retrieved successfully",
                Data = learner
            });
        }

        // this gets the currently logged in learner's own profile
        [Authorize(Roles = "Learner")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var learnerId = GetLoggedInLearnerId();

            if (learnerId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners
                .Include(x => x.Instructor)
                .Where(x => x.LearnerId == learnerId.Value)
                .Select(x => new LearnerResponseDto
                {
                    LearnerId = x.LearnerId,
                    LearnerName = x.LearnerName,
                    LearnerLicenceId = x.LearnerLicenceId,
                    LearnerEmail = x.LearnerEmail,
                    LearnerPhone = x.LearnerPhone,
                    LearnerStatus = x.LearnerStatus,
                    PastLessonCount = x.PastLessonCount,
                    NextLessonCount = x.NextLessonCount,
                    LearnerLessonType = x.LearnerLessonType,
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : null
                })
                .FirstOrDefaultAsync();

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<LearnerResponseDto>
            {
                Success = true,
                Message = "learner profile retrieved successfully",
                Data = learner
            });
        }

        // this updates the logged in learner's account details
        [Authorize(Roles = "Learner")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateLearner([FromBody] UpdateLearnerRequest request)
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

            var learnerId = GetLoggedInLearnerId();

            if (learnerId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners.FirstOrDefaultAsync(x => x.LearnerId == learnerId.Value);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            var learnerName = request.LearnerName?.Trim() ?? "";
            var learnerEmail = request.LearnerEmail?.Trim() ?? "";
            var learnerPhone = request.LearnerPhone?.Trim() ?? "";
            var learnerLessonType = request.LearnerLessonType?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(learnerName) ||
                string.IsNullOrWhiteSpace(learnerEmail) ||
                string.IsNullOrWhiteSpace(learnerPhone) ||
                string.IsNullOrWhiteSpace(learnerLessonType) ||
                request.InstructorId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "all learner fields are required",
                    Data = null
                });
            }

            if (!new EmailAddressAttribute().IsValid(learnerEmail))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid email format",
                    Data = null
                });
            }

            if (learnerLessonType != "Manual" && learnerLessonType != "Automatic")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "lesson type must be Manual or Automatic",
                    Data = null
                });
            }

            var emailExists = await _context.Learners
                .AnyAsync(x => x.LearnerEmail.ToLower() == learnerEmail.ToLower() && x.LearnerId != learnerId.Value);

            if (emailExists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "email already exists",
                    Data = null
                });
            }

            var selectedInstructor = await _context.Instructors
                .FirstOrDefaultAsync(x => x.InstructorId == request.InstructorId);

            if (selectedInstructor == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "selected instructor does not exist",
                    Data = null
                });
            }

            if (learnerLessonType == "Manual" && selectedInstructor.InstructorCarType != "Manual")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "manual learners can only choose manual instructors",
                    Data = null
                });
            }

            learner.LearnerName = learnerName;
            learner.LearnerEmail = learnerEmail;
            learner.LearnerPhone = learnerPhone;
            learner.LearnerLessonType = learnerLessonType;
            learner.InstructorId = request.InstructorId;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "learner updated successfully",
                Data = null
            });
        }

        // this changes the logged in learner's password after checking the current password
        [Authorize(Roles = "Learner")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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

            var learnerId = GetLoggedInLearnerId();

            if (learnerId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners.FirstOrDefaultAsync(x => x.LearnerId == learnerId.Value);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            var currentPassword = request.CurrentPassword?.Trim() ?? "";
            var newPassword = request.NewPassword?.Trim() ?? "";
            var confirmPassword = request.ConfirmPassword?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "all password fields are required",
                    Data = null
                });
            }

            if (!IsValidPassword(newPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "new password must be at least 6 characters and include one uppercase letter and one number",
                    Data = null
                });
            }

            if (newPassword != confirmPassword)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "new passwords do not match",
                    Data = null
                });
            }

            var validCurrentPassword = _passwordService.VerifyPassword(
                learner.LearnerPasswordHash,
                currentPassword
            );

            if (!validCurrentPassword)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "current password is incorrect",
                    Data = null
                });
            }

            learner.LearnerPasswordHash = _passwordService.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "password changed successfully",
                Data = null
            });
        }

        // this deletes the logged in learner account after confirming the password
        [Authorize(Roles = "Learner")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteLearnerRequest request)
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

            var learnerId = GetLoggedInLearnerId();

            if (learnerId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners
                .Include(x => x.DbBookings)
                .Include(x => x.StudentRequests)
                .FirstOrDefaultAsync(x => x.LearnerId == learnerId.Value);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            var password = request.Password?.Trim() ?? "";
            var confirmText = request.ConfirmText?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmText))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "password and confirmation text are required",
                    Data = null
                });
            }

            if (confirmText != "DELETE")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "type DELETE to confirm account removal",
                    Data = null
                });
            }

            var validPassword = _passwordService.VerifyPassword(
                learner.LearnerPasswordHash,
                password
            );

            if (!validPassword)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "password is incorrect",
                    Data = null
                });
            }

            if (learner.DbBookings.Any())
            {
                _context.Bookings.RemoveRange(learner.DbBookings);
            }

            if (learner.StudentRequests.Any())
            {
                _context.StudentRequests.RemoveRange(learner.StudentRequests);
            }

            _context.Learners.Remove(learner);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "account deleted successfully",
                Data = null
            });
        }

        // this reads the learner id from the jwt token
        private int? GetLoggedInLearnerId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
            {
                return null;
            }

            return learnerId;
        }

        // this validates password strength
        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Length >= 6 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit);
        }
    }

    public class LearnerResponseDto
    {
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = "";
        public string LearnerLicenceId { get; set; } = "";
        public string LearnerEmail { get; set; } = "";
        public string LearnerPhone { get; set; } = "";
        public string LearnerStatus { get; set; } = "";
        public int PastLessonCount { get; set; }
        public int NextLessonCount { get; set; }
        public string LearnerLessonType { get; set; } = "";
        public int InstructorId { get; set; }
        public string? InstructorName { get; set; }
    }

    public class UpdateLearnerRequest
    {
        public string LearnerName { get; set; } = "";
        public string LearnerEmail { get; set; } = "";
        public string LearnerPhone { get; set; } = "";
        public string LearnerLessonType { get; set; } = "";
        public int InstructorId { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }

    public class DeleteLearnerRequest
    {
        public string Password { get; set; } = "";
        public string ConfirmText { get; set; } = "";
    }
}