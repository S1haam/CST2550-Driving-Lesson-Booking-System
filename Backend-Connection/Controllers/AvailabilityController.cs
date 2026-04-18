using Backend_Connection.Data;
using Backend_Connection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // this gives the controller access to the database
        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // this gets all availability records from the database
        [HttpGet]
        public async Task<IActionResult> GetAllAvailability()
        {
            var data = await _context.Availabilities
                .Include(x => x.Instructor)
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
                Message = "availabilities retrieved successfully",
                Data = data
            });
        }

        // this gets available untaken slots for a specific instructor
        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetAvailabilityByInstructor(int instructorId)
        {
            var instructorExists = await _context.Instructors
                .AnyAsync(x => x.InstructorId == instructorId);

            if (!instructorExists)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "instructor not found",
                    Data = null
                });
            }

            var data = await _context.Availabilities
                .Include(x => x.Instructor)
                .Where(x => x.InstructorId == instructorId && !x.IsTaken)
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
                Message = "instructor availability retrieved successfully",
                Data = data
            });
        }
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