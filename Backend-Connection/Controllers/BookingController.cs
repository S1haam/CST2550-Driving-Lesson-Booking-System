using Backend_Connection.Data;
using Backend_Connection.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // this gives the controller access to the database
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // this gets all booking records from the database
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var data = await _context.Bookings
                .Include(x => x.Learner)
                .Include(x => x.Instructor)
                .Select(x => new BookingResponseDto
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

            return Ok(new ApiResponse<List<BookingResponseDto>>
            {
                Success = true,
                Message = "bookings retrieved successfully",
                Data = data
            });
        }

        // this gets one booking by id for the logged in learner
        [Authorize(Roles = "Learner")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
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
                .FirstOrDefaultAsync(x => x.BookingId == id && x.LearnerId == learnerId);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "booking not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<BookingResponseDto>
            {
                Success = true,
                Message = "booking retrieved successfully",
                Data = new BookingResponseDto
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
                }
            });
        }

        // this creates a new booking for the logged in learner using an available slot
        [Authorize(Roles = "Learner")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
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

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            if (request.AvailabilityId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "availability id is required",
                    Data = null
                });
            }

            var learner = await _context.Learners
                .FirstOrDefaultAsync(x => x.LearnerId == learnerId);

            if (learner == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "learner not found",
                    Data = null
                });
            }

            var availability = await _context.Availabilities
                .Include(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.AvailabilityId == request.AvailabilityId);

            if (availability == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "availability slot not found",
                    Data = null
                });
            }

            if (availability.IsTaken)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "this slot has already been taken",
                    Data = null
                });
            }

            var lessonType = request.LessonType?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(lessonType))
            {
                lessonType = "Beginners";
            }

            var booking = new Booking
            {
                LearnerId = learner.LearnerId,
                InstructorId = availability.InstructorId,
                LessonDate = availability.AvailableDateTime.Date,
                LessonTime = availability.AvailableDateTime.TimeOfDay,
                LessonType = lessonType,
                BookingStatus = "Confirmed",
                InstructorNotes = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);

            availability.IsTaken = true;

            learner.NextLessonCount += 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<BookingResponseDto>
            {
                Success = true,
                Message = "booking created successfully",
                Data = new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    LearnerId = booking.LearnerId,
                    LearnerName = learner.LearnerName,
                    InstructorId = booking.InstructorId,
                    InstructorName = availability.Instructor != null ? availability.Instructor.InstructorName : "",
                    LessonDate = booking.LessonDate,
                    LessonTime = booking.LessonTime,
                    LessonType = booking.LessonType,
                    BookingStatus = booking.BookingStatus,
                    InstructorNotes = booking.InstructorNotes,
                    CreatedAt = booking.CreatedAt,
                    UpdatedAt = booking.UpdatedAt
                }
            });
        }

        // this cancels a booking for the logged in learner
        [Authorize(Roles = "Learner")]
        [HttpPut("cancel/{id:int}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == id && x.LearnerId == learnerId);

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
                .FirstOrDefaultAsync(x => x.LearnerId == learnerId);

            if (learner != null && learner.NextLessonCount > 0)
            {
                learner.NextLessonCount -= 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "booking cancelled successfully",
                Data = null
            });
        }

        // this reschedules a booking for the logged in learner
        [Authorize(Roles = "Learner")]
        [HttpPut("reschedule/{id:int}")]
        public async Task<IActionResult> RescheduleBooking(int id, [FromBody] RescheduleBookingRequest request)
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

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "invalid token",
                    Data = null
                });
            }

            if (request.NewAvailabilityId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "new availability id is required",
                    Data = null
                });
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == id && x.LearnerId == learnerId);

            if (booking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "booking not found",
                    Data = null
                });
            }

            if (booking.BookingStatus != "Confirmed")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "only confirmed bookings can be rescheduled",
                    Data = null
                });
            }

            var newAvailability = await _context.Availabilities
                .Include(x => x.Instructor)
                .FirstOrDefaultAsync(x => x.AvailabilityId == request.NewAvailabilityId);

            if (newAvailability == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "new availability slot not found",
                    Data = null
                });
            }

            if (newAvailability.IsTaken)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "the selected slot has already been taken",
                    Data = null
                });
            }

            var oldAvailability = await _context.Availabilities
                .FirstOrDefaultAsync(x =>
                    x.InstructorId == booking.InstructorId &&
                    x.AvailableDateTime.Date == booking.LessonDate.Date &&
                    x.AvailableDateTime.TimeOfDay == booking.LessonTime);

            if (oldAvailability != null)
            {
                oldAvailability.IsTaken = false;
            }

            booking.InstructorId = newAvailability.InstructorId;
            booking.LessonDate = newAvailability.AvailableDateTime.Date;
            booking.LessonTime = newAvailability.AvailableDateTime.TimeOfDay;
            booking.UpdatedAt = DateTime.Now;

            newAvailability.IsTaken = true;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<BookingResponseDto>
            {
                Success = true,
                Message = "booking rescheduled successfully",
                Data = new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    LearnerId = booking.LearnerId,
                    LearnerName = "",
                    InstructorId = booking.InstructorId,
                    InstructorName = newAvailability.Instructor != null ? newAvailability.Instructor.InstructorName : "",
                    LessonDate = booking.LessonDate,
                    LessonTime = booking.LessonTime,
                    LessonType = booking.LessonType,
                    BookingStatus = booking.BookingStatus,
                    InstructorNotes = booking.InstructorNotes,
                    CreatedAt = booking.CreatedAt,
                    UpdatedAt = booking.UpdatedAt
                }
            });
        }

        // this gets all upcoming bookings for the logged in learner
        [Authorize(Roles = "Learner")]
        [HttpGet("learner/upcoming")]
        public async Task<IActionResult> GetUpcomingBookings()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
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
                .Where(x => x.LearnerId == learnerId &&
                            x.BookingStatus == "Confirmed" &&
                            x.LessonDate.Add(x.LessonTime) >= now)
                .OrderBy(x => x.LessonDate)
                .ThenBy(x => x.LessonTime)
                .Select(x => new BookingResponseDto
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

            return Ok(new ApiResponse<List<BookingResponseDto>>
            {
                Success = true,
                Message = "upcoming bookings retrieved successfully",
                Data = data
            });
        }

        // this gets all past bookings for the logged in learner
        [Authorize(Roles = "Learner")]
        [HttpGet("learner/past")]
        public async Task<IActionResult> GetPastBookings()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int learnerId))
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
                .Where(x => x.LearnerId == learnerId &&
                            x.BookingStatus == "Confirmed" &&
                            x.LessonDate.Add(x.LessonTime) < now)
                .OrderByDescending(x => x.LessonDate)
                .ThenByDescending(x => x.LessonTime)
                .Select(x => new BookingResponseDto
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

            return Ok(new ApiResponse<List<BookingResponseDto>>
            {
                Success = true,
                Message = "past bookings retrieved successfully",
                Data = data
            });
        }
    }

    public class CreateBookingRequest
    {
        public int AvailabilityId { get; set; }
        public string LessonType { get; set; } = "";
    }

    public class RescheduleBookingRequest
    {
        public int NewAvailabilityId { get; set; }
    }

    public class BookingResponseDto
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
}