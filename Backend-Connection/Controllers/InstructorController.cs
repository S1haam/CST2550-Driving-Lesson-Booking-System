using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        // this gives the controller access to the database and password service
        public InstructorController(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // this gets all instructors from the database
        [HttpGet]
        public async Task<IActionResult> GetAllInstructors()
        {
            var data = await _context.Instructors
                .Select(x => new InstructorResponseDto
                {
                    InstructorId = x.InstructorId,
                    InstructorCode = x.InstructorCode,
                    InstructorName = x.InstructorName,
                    InstructorEmail = x.InstructorEmail,
                    InstructorPhone = x.InstructorPhone,
                    InstructorCarType = x.InstructorCarType,
                    InstructorStatus = x.InstructorStatus
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorResponseDto>>
            {
                Success = true,
                Message = "instructors retrieved successfully",
                Data = data
            });
        }

        // this gets one instructor by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            var instructor = await _context.Instructors
                .Where(x => x.InstructorId == id)
                .Select(x => new InstructorResponseDto
                {
                    InstructorId = x.InstructorId,
                    InstructorCode = x.InstructorCode,
                    InstructorName = x.InstructorName,
                    InstructorEmail = x.InstructorEmail,
                    InstructorPhone = x.InstructorPhone,
                    InstructorCarType = x.InstructorCarType,
                    InstructorStatus = x.InstructorStatus
                })
                .FirstOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<InstructorResponseDto>
            {
                Success = true,
                Message = "instructor retrieved successfully",
                Data = instructor
            });
        }

        // this gets instructors based on the learner's selected lesson type
        [HttpGet("by-lesson-type/{lessonType}")]
        public async Task<IActionResult> GetInstructorsByLessonType(string lessonType)
        {
            var cleanLessonType = lessonType?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(cleanLessonType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "lesson type is required",
                    Data = null
                });
            }

            IQueryable<Instructor> query = _context.Instructors;

            if (cleanLessonType == "Manual")
            {
                query = query.Where(x => x.InstructorCarType == "Manual");
            }
            else if (cleanLessonType == "Automatic")
            {
                query = query.Where(x => x.InstructorCarType == "Automatic" || x.InstructorCarType == "Manual");
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid lesson type",
                    Data = null
                });
            }

            var data = await query
                .Select(x => new InstructorResponseDto
                {
                    InstructorId = x.InstructorId,
                    InstructorCode = x.InstructorCode,
                    InstructorName = x.InstructorName,
                    InstructorEmail = x.InstructorEmail,
                    InstructorPhone = x.InstructorPhone,
                    InstructorCarType = x.InstructorCarType,
                    InstructorStatus = x.InstructorStatus
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorResponseDto>>
            {
                Success = true,
                Message = "filtered instructors retrieved successfully",
                Data = data
            });
        }

        // this gets the currently logged in instructor profile
        [Authorize(Roles = "Instructor")]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentInstructor()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var instructor = await _context.Instructors
                .Where(x => x.InstructorId == instructorId.Value)
                .Select(x => new InstructorResponseDto
                {
                    InstructorId = x.InstructorId,
                    InstructorCode = x.InstructorCode,
                    InstructorName = x.InstructorName,
                    InstructorEmail = x.InstructorEmail,
                    InstructorPhone = x.InstructorPhone,
                    InstructorCarType = x.InstructorCarType,
                    InstructorStatus = x.InstructorStatus
                })
                .FirstOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<InstructorResponseDto>
            {
                Success = true,
                Message = "instructor profile retrieved successfully",
                Data = instructor
            });
        }

        // this updates the logged in instructor account details
        [Authorize(Roles = "Instructor")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateInstructor([FromBody] UpdateInstructorRequest request)
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

            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var instructor = await _context.Instructors.FirstOrDefaultAsync(x => x.InstructorId == instructorId.Value);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            var instructorName = request.InstructorName?.Trim() ?? "";
            var instructorEmail = request.InstructorEmail?.Trim() ?? "";
            var instructorPhone = request.InstructorPhone?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(instructorName) ||
                string.IsNullOrWhiteSpace(instructorEmail) ||
                string.IsNullOrWhiteSpace(instructorPhone))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "all instructor fields are required",
                    Data = null
                });
            }

            if (!new EmailAddressAttribute().IsValid(instructorEmail))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid email format",
                    Data = null
                });
            }

            var emailExists = await _context.Instructors
                .AnyAsync(x => x.InstructorEmail.ToLower() == instructorEmail.ToLower() &&
                               x.InstructorId != instructorId.Value);

            if (emailExists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "email already exists",
                    Data = null
                });
            }

            instructor.InstructorName = instructorName;
            instructor.InstructorEmail = instructorEmail;
            instructor.InstructorPhone = instructorPhone;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "instructor updated successfully",
                Data = null
            });
        }

        // this changes the logged in instructor password after checking the current password
        [Authorize(Roles = "Instructor")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeInstructorPasswordRequest request)
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

            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var instructor = await _context.Instructors.FirstOrDefaultAsync(x => x.InstructorId == instructorId.Value);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
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
                instructor.InstructorPasswordHash ?? "",
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

            instructor.InstructorPasswordHash = _passwordService.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "password changed successfully",
                Data = null
            });
        }

        // this updates the logged in instructor status
        [Authorize(Roles = "Instructor")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateInstructorStatusRequest request)
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

            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var instructor = await _context.Instructors.FirstOrDefaultAsync(x => x.InstructorId == instructorId.Value);

            if (instructor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            var instructorStatus = request.InstructorStatus?.Trim() ?? "";

            if (instructorStatus != "Active" && instructorStatus != "Inactive")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "status must be Active or Inactive",
                    Data = null
                });
            }

            instructor.InstructorStatus = instructorStatus;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "status updated successfully",
                Data = null
            });
        }

        // this gets inbox items for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var cancelledItems = await _context.Bookings
                .Where(x => x.InstructorId == instructorId.Value && x.BookingStatus == "Cancelled")
                .GroupBy(x => x.LearnerId)
                .Select(x => x
                    .OrderByDescending(b => b.UpdatedAt)
                    .FirstOrDefault()!)
                .Select(x => new InstructorInboxItemDto
                {
                    Id = x.BookingId,
                    Title = "cancelled booking",
                    Type = "cancel",
                    CreatedAt = x.UpdatedAt,
                    BookingId = x.BookingId,
                    LearnerId = x.LearnerId
                })
                .ToListAsync();

            var removedItems = await _context.Learners
                .Where(x => x.InstructorId == instructorId.Value && x.LearnerStatus == "Removed")
                .Select(x => new InstructorInboxItemDto
                {
                    Id = x.LearnerId,
                    Title = "removed student",
                    Type = "removed",
                    CreatedAt = DateTime.MinValue,
                    BookingId = null,
                    LearnerId = x.LearnerId
                })
                .ToListAsync();

            var data = cancelledItems
                .Concat(removedItems)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Ok(new ApiResponse<List<InstructorInboxItemDto>>
            {
                Success = true,
                Message = "inbox items retrieved successfully",
                Data = data
            });
        }

        // this gets notifications for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var data = await _context.Notifications
                .Where(x => x.InstructorId == instructorId.Value)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationDto
                {
                    NotificationId = x.NotificationId,
                    InstructorId = x.InstructorId,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    CreatedAt = x.CreatedAt,
                    IsRead = x.IsRead,
                    IsPinned = x.IsPinned,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<NotificationDto>>
            {
                Success = true,
                Message = "notifications retrieved successfully",
                Data = data
            });
        }

        // this gets one notification by id for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("notification/{id:int}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var notification = await _context.Notifications
                .Where(x => x.NotificationId == id && x.InstructorId == instructorId.Value)
                .Select(x => new NotificationDto
                {
                    NotificationId = x.NotificationId,
                    InstructorId = x.InstructorId,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    CreatedAt = x.CreatedAt,
                    IsRead = x.IsRead,
                    IsPinned = x.IsPinned,
                    IsDeleted = x.IsDeleted
                })
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "notification not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<NotificationDto>
            {
                Success = true,
                Message = "notification retrieved successfully",
                Data = notification
            });
        }

        // this pins a notification for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("notification/{id:int}/pin")]
        public async Task<IActionResult> PinNotification(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.InstructorId == instructorId.Value);

            if (notification == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "notification not found",
                    Data = null
                });
            }

            notification.IsPinned = true;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "notification pinned successfully",
                Data = null
            });
        }

        // this unpins a notification for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("notification/{id:int}/unpin")]
        public async Task<IActionResult> UnpinNotification(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.InstructorId == instructorId.Value);

            if (notification == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "notification not found",
                    Data = null
                });
            }

            notification.IsPinned = false;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "notification unpinned successfully",
                Data = null
            });
        }

        // this soft deletes a notification for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("notification/{id:int}/delete")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.InstructorId == instructorId.Value);

            if (notification == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "notification not found",
                    Data = null
                });
            }

            notification.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "notification deleted successfully",
                Data = null
            });
        }

        // this restores a deleted notification for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("notification/{id:int}/restore")]
        public async Task<IActionResult> RestoreNotification(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.InstructorId == instructorId.Value);

            if (notification == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "notification not found",
                    Data = null
                });
            }

            notification.IsDeleted = false;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "notification restored successfully",
                Data = null
            });
        }

        // this gets the learners linked to the logged in instructor for instructor home
        [Authorize(Roles = "Instructor")]
        [HttpGet("home/students")]
        public async Task<IActionResult> GetHomeStudents()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var data = await _context.Learners
                .Where(x => x.InstructorId == instructorId.Value && x.LearnerStatus != "Removed")
                .OrderBy(x => x.LearnerName)
                .Select(x => new InstructorHomeStudentDto
                {
                    LearnerId = x.LearnerId,
                    LearnerName = x.LearnerName
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorHomeStudentDto>>
            {
                Success = true,
                Message = "students retrieved successfully",
                Data = data
            });
        }

        // this gets one learner linked to the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("learner/{id:int}")]
        public async Task<IActionResult> GetInstructorLearnerById(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners
                .Where(x => x.LearnerId == id && x.InstructorId == instructorId.Value)
                .Select(x => new InstructorLearnerDetailsDto
                {
                    LearnerId = x.LearnerId,
                    LearnerName = x.LearnerName,
                    LearnerLicenceId = x.LearnerLicenceId,
                    LearnerEmail = x.LearnerEmail,
                    LearnerPhone = x.LearnerPhone,
                    LearnerStatus = x.LearnerStatus,
                    PastLessonCount = x.PastLessonCount,
                    NextLessonCount = x.NextLessonCount
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

            return Ok(new ApiResponse<InstructorLearnerDetailsDto>
            {
                Success = true,
                Message = "learner retrieved successfully",
                Data = learner
            });
        }

        // this marks a learner as removed from the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("learner/{id:int}/remove")]
        public async Task<IActionResult> RemoveLearner(int id)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var learner = await _context.Learners
                .FirstOrDefaultAsync(x => x.LearnerId == id && x.InstructorId == instructorId.Value);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            learner.LearnerStatus = "Removed";

            _context.Notifications.Add(new Notification
            {
                InstructorId = instructorId.Value,
                Message = $"Student {learner.LearnerName} has been removed.",
                NotificationType = "RemovedStudent",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsPinned = false,
                IsDeleted = false
            });

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "learner removed successfully",
                Data = null
            });
        }

        // this gets upcoming lessons for the logged in instructor home page
        [Authorize(Roles = "Instructor")]
        [HttpGet("home/upcoming-lessons")]
        public async Task<IActionResult> GetUpcomingLessonsForInstructor()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var now = DateTime.Now;

            var data = await _context.Bookings
                .Include(x => x.Learner)
                .Where(x => x.InstructorId == instructorId.Value &&
                            x.BookingStatus == "Confirmed" &&
                            x.LessonDate.Add(x.LessonTime) >= now)
                .OrderBy(x => x.LessonDate)
                .ThenBy(x => x.LessonTime)
                .Select(x => new InstructorHomeLessonDto
                {
                    Id = x.BookingId,
                    LearnerName = x.Learner != null ? x.Learner.LearnerName : "",
                    LessonDate = x.LessonDate,
                    LessonTime = x.LessonTime,
                    DisplayText = x.LessonDate.ToString("ddd, dd/MM/yy") + ", " + x.LessonTime.ToString(@"hh\:mm")
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorHomeLessonDto>>
            {
                Success = true,
                Message = "upcoming lessons retrieved successfully",
                Data = data
            });
        }

        // this gets upcoming lessons for the logged in instructor schedule page
        [Authorize(Roles = "Instructor")]
        [HttpGet("schedule/upcoming")]
        public async Task<IActionResult> GetUpcomingScheduleLessons()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var now = DateTime.Now;

            var data = await _context.Bookings
                .Include(x => x.Learner)
                .Include(x => x.Instructor)
                .Where(x => x.InstructorId == instructorId.Value &&
                            x.BookingStatus == "Confirmed" &&
                            x.LessonDate.Add(x.LessonTime) >= now)
                .OrderBy(x => x.LessonDate)
                .ThenBy(x => x.LessonTime)
                .Select(x => new InstructorScheduleLessonDto
                {
                    BookingId = x.BookingId,
                    LearnerId = x.LearnerId,
                    LearnerName = x.Learner != null ? x.Learner.LearnerName : "",
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : "",
                    LessonDate = x.LessonDate,
                    LessonTime = x.LessonTime,
                    LessonType = x.LessonType,
                    BookingStatus = x.BookingStatus,
                    InstructorNotes = x.InstructorNotes,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorScheduleLessonDto>>
            {
                Success = true,
                Message = "upcoming schedule lessons retrieved successfully",
                Data = data
            });
        }

        // this gets past lessons for the logged in instructor schedule page
        [Authorize(Roles = "Instructor")]
        [HttpGet("schedule/past")]
        public async Task<IActionResult> GetPastScheduleLessons()
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var now = DateTime.Now;

            var data = await _context.Bookings
                .Include(x => x.Learner)
                .Include(x => x.Instructor)
                .Where(x => x.InstructorId == instructorId.Value &&
                            x.BookingStatus == "Confirmed" &&
                            x.LessonDate.Add(x.LessonTime) < now)
                .OrderByDescending(x => x.LessonDate)
                .ThenByDescending(x => x.LessonTime)
                .Select(x => new InstructorScheduleLessonDto
                {
                    BookingId = x.BookingId,
                    LearnerId = x.LearnerId,
                    LearnerName = x.Learner != null ? x.Learner.LearnerName : "",
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : "",
                    LessonDate = x.LessonDate,
                    LessonTime = x.LessonTime,
                    LessonType = x.LessonType,
                    BookingStatus = x.BookingStatus,
                    InstructorNotes = x.InstructorNotes,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorScheduleLessonDto>>
            {
                Success = true,
                Message = "past schedule lessons retrieved successfully",
                Data = data
            });
        }

        // this gets one booking by id for the logged in instructor schedule pages
        [Authorize(Roles = "Instructor")]
        [HttpGet("schedule/booking/{bookingId:int}")]
        public async Task<IActionResult> GetScheduleBookingById(int bookingId)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .Include(x => x.Learner)
                .Include(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId.Value);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "booking not found",
                    Data = null
                });
            }

            var data = new InstructorScheduleLessonDto
            {
                BookingId = booking.BookingId,
                LearnerId = booking.LearnerId,
                LearnerName = booking.Learner != null ? booking.Learner.LearnerName : "",
                InstructorId = booking.InstructorId,
                InstructorName = booking.Instructor != null ? booking.Instructor.InstructorName : "",
                LessonDate = booking.LessonDate,
                LessonTime = booking.LessonTime,
                LessonType = booking.LessonType,
                BookingStatus = booking.BookingStatus,
                InstructorNotes = booking.InstructorNotes,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return Ok(new ApiResponse<InstructorScheduleLessonDto>
            {
                Success = true,
                Message = "schedule booking retrieved successfully",
                Data = data
            });
        }

        // this cancels a booking for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPut("schedule/cancel/{bookingId:int}")]
        public async Task<IActionResult> CancelScheduleBooking(int bookingId)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .Include(x => x.Learner)
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId.Value);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "booking not found",
                    Data = null
                });
            }

            if (booking.BookingStatus == "Cancelled")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "booking is already cancelled",
                    Data = null
                });
            }

            booking.BookingStatus = "Cancelled";
            booking.UpdatedAt = DateTime.Now;

            var matchingAvailability = await _context.Availabilities
                .FirstOrDefaultAsync(x =>
                    x.InstructorId == booking.InstructorId &&
                    x.AvailableDateTime.Date == booking.LessonDate.Date &&
                    x.AvailableDateTime.TimeOfDay == booking.LessonTime);

            if (matchingAvailability != null)
            {
                matchingAvailability.IsTaken = false;
            }

            var learner = await _context.Learners
                .FirstOrDefaultAsync(x => x.LearnerId == booking.LearnerId);

            if (learner != null && learner.NextLessonCount > 0)
            {
                learner.NextLessonCount -= 1;
            }

            _context.Notifications.Add(new Notification
            {
                InstructorId = instructorId.Value,
                Message = booking.Learner != null
                    ? $"Booking with {booking.Learner.LearnerName} has been cancelled."
                    : "A booking has been cancelled.",
                NotificationType = "CancelledBooking",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsPinned = false,
                IsDeleted = false
            });

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "booking cancelled successfully",
                Data = null
            });
        }

        // this gets one lesson detail record for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("lesson-details/{bookingId:int}")]
        public async Task<IActionResult> GetInstructorLessonDetails(int bookingId)
        {
            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .Include(x => x.Learner)
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId.Value);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "lesson not found",
                    Data = null
                });
            }

            var data = new InstructorLessonDetailsDto
            {
                BookingId = booking.BookingId,
                LearnerId = booking.LearnerId,
                LearnerName = booking.Learner != null ? booking.Learner.LearnerName : "",
                LessonDate = booking.LessonDate,
                LessonTime = booking.LessonTime,
                BookingStatus = booking.BookingStatus,
                InstructorNotes = booking.InstructorNotes ?? ""
            };

            return Ok(new ApiResponse<InstructorLessonDetailsDto>
            {
                Success = true,
                Message = "lesson details retrieved successfully",
                Data = data
            });
        }

        // this saves instructor notes for a lesson
        [Authorize(Roles = "Instructor")]
        [HttpPut("lesson-details/{bookingId:int}/notes")]
        public async Task<IActionResult> SaveInstructorNotes(int bookingId, [FromBody] UpdateInstructorNotesDto request)
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

            var instructorId = GetLoggedInInstructorId();

            if (instructorId == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId.Value);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "lesson not found",
                    Data = null
                });
            }

            booking.InstructorNotes = request.InstructorNotes?.Trim() ?? "";
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "instructor notes saved successfully",
                Data = null
            });
        }

        // this reads the instructor id from the jwt token
        private int? GetLoggedInInstructorId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
            {
                return null;
            }

            return instructorId;
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

    public class InstructorResponseDto
    {
        public int InstructorId { get; set; }
        public string InstructorCode { get; set; } = "";
        public string InstructorName { get; set; } = "";
        public string InstructorEmail { get; set; } = "";
        public string InstructorPhone { get; set; } = "";
        public string InstructorCarType { get; set; } = "";
        public string InstructorStatus { get; set; } = "";
    }

    public class UpdateInstructorRequest
    {
        public string InstructorName { get; set; } = "";
        public string InstructorEmail { get; set; } = "";
        public string InstructorPhone { get; set; } = "";
    }

    public class ChangeInstructorPasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }

    public class UpdateInstructorStatusRequest
    {
        public string InstructorStatus { get; set; } = "";
    }

    public class InstructorInboxItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int? BookingId { get; set; }
        public int? LearnerId { get; set; }
    }

    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int InstructorId { get; set; }
        public string Message { get; set; } = "";
        public string NotificationType { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class InstructorHomeLessonDto
    {
        public int Id { get; set; }
        public string LearnerName { get; set; } = "";
        public DateTime LessonDate { get; set; }
        public TimeSpan LessonTime { get; set; }
        public string DisplayText { get; set; } = "";
    }

    public class InstructorHomeStudentDto
    {
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = "";
    }

    public class InstructorLearnerDetailsDto
    {
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = "";
        public string LearnerLicenceId { get; set; } = "";
        public string LearnerEmail { get; set; } = "";
        public string LearnerPhone { get; set; } = "";
        public string LearnerStatus { get; set; } = "";
        public int PastLessonCount { get; set; }
        public int NextLessonCount { get; set; }
    }

    public class InstructorScheduleLessonDto
    {
        public int BookingId { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = "";
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = "";
        public DateTime LessonDate { get; set; }
        public TimeSpan LessonTime { get; set; }
        public string LessonType { get; set; } = "";
        public string BookingStatus { get; set; } = "";
        public string? InstructorNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class InstructorLessonDetailsDto
    {
        public int BookingId { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = "";
        public DateTime LessonDate { get; set; }
        public TimeSpan LessonTime { get; set; }
        public string BookingStatus { get; set; } = "";
        public string InstructorNotes { get; set; } = "";
    }

    public class UpdateInstructorNotesDto
    {
        public string InstructorNotes { get; set; } = "";
    }
}