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
    public class AvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // gets all future untaken availability slots for a specific instructor
        [HttpGet("instructor/{instructorId:int}")]
        public async Task<IActionResult> GetAvailabilityForInstructor(int instructorId)
        {
            var now = DateTime.Now;

            var data = await _context.Availabilities
                .Include(x => x.Instructor)
                .Where(x => x.InstructorId == instructorId &&
                            !x.IsTaken &&
                            x.AvailableDateTime > now)
                .OrderBy(x => x.AvailableDateTime)
                .Select(x => new AvailabilityResponseDto
                {
                    AvailabilityId = x.AvailabilityId,
                    AvailableDateTime = x.AvailableDateTime,
                    IsTaken = x.IsTaken,
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor != null ? x.Instructor.InstructorName : ""
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AvailabilityResponseDto>>
            {
                Success = true,
                Message = "availability retrieved successfully",
                Data = data
            });
        }

        // gets all availability slots for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor/my-slots")]
        public async Task<IActionResult> GetMyAvailabilitySlots()
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

            var data = await _context.Availabilities
                .Where(x => x.InstructorId == instructorId)
                .OrderBy(x => x.AvailableDateTime)
                .Select(x => new InstructorAvailabilityDto
                {
                    AvailabilityId = x.AvailabilityId,
                    InstructorId = x.InstructorId,
                    AvailableDateTime = x.AvailableDateTime,
                    IsTaken = x.IsTaken
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<InstructorAvailabilityDto>>
            {
                Success = true,
                Message = "availability retrieved successfully",
                Data = data
            });
        }

        // creates a new availability slot for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpPost("instructor/my-slots")]
        public async Task<IActionResult> CreateMyAvailabilitySlot([FromBody] CreateAvailabilitySlotRequest request)
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

            if (request.AvailableDateTime == default)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "valid date and time are required",
                    Data = null
                });
            }

            if (request.AvailableDateTime <= DateTime.Now)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "availability must be in the future",
                    Data = null
                });
            }

            var alreadyExists = await _context.Availabilities.AnyAsync(x =>
                x.InstructorId == instructorId &&
                x.AvailableDateTime == request.AvailableDateTime);

            if (alreadyExists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "this availability slot already exists",
                    Data = null
                });
            }

            var availability = new Availability
            {
                InstructorId = instructorId,
                AvailableDateTime = request.AvailableDateTime,
                IsTaken = false
            };

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<InstructorAvailabilityDto>
            {
                Success = true,
                Message = "availability slot created successfully",
                Data = new InstructorAvailabilityDto
                {
                    AvailabilityId = availability.AvailabilityId,
                    InstructorId = availability.InstructorId,
                    AvailableDateTime = availability.AvailableDateTime,
                    IsTaken = availability.IsTaken
                }
            });
        }

        // deletes one availability slot for the logged in instructor
        [Authorize(Roles = "Instructor")]
        [HttpDelete("instructor/my-slots/{availabilityId:int}")]
        public async Task<IActionResult> DeleteMyAvailabilitySlot(int availabilityId)
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

            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(x => x.AvailabilityId == availabilityId && x.InstructorId == instructorId);

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
                    Message = "cannot remove a slot that has already been booked",
                    Data = null
                });
            }

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "availability slot removed successfully",
                Data = null
            });
        }
    }

    public class CreateAvailabilitySlotRequest
    {
        public DateTime AvailableDateTime { get; set; }
    }

    public class InstructorAvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public int InstructorId { get; set; }
        public DateTime AvailableDateTime { get; set; }
        public bool IsTaken { get; set; }
    }

    public class AvailabilityResponseDto
    {
        public int AvailabilityId { get; set; }
        public DateTime AvailableDateTime { get; set; }
        public bool IsTaken { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = "";
    }
}