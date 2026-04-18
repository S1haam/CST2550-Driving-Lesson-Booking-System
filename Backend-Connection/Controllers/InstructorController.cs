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
        [HttpGet("{id}")]
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
}