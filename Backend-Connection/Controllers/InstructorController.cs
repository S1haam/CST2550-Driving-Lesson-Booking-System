using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend_Connection.Data;
using Backend_Connection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // this gives the controller access to the database
        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var instructor = await _context.Instructors
                .Where(x => x.InstructorId == instructorId)
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

        // this gets the learners linked to the logged in instructor for instructor home
        [Authorize(Roles = "Instructor")]
        [HttpGet("home/students")]
        public async Task<IActionResult> GetHomeStudents()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var data = await _context.Learners
                .Where(x => x.InstructorId == instructorId)
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

        // this gets upcoming lessons for the logged in instructor home page
        [Authorize(Roles = "Instructor")]
        [HttpGet("home/upcoming-lessons")]
        public async Task<IActionResult> GetUpcomingLessonsForInstructor()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
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
                .Where(x => x.InstructorId == instructorId &&
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

        // this gets one lesson detail record for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("lesson-details/{bookingId:int}")]
        public async Task<IActionResult> GetInstructorLessonDetails(int bookingId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
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
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId);

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

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int instructorId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.InstructorId == instructorId);

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