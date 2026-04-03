using Backend_Connection.Data;
using Backend_Connection.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Connection.Controllers
{

    // enables automatic model validation and consistent API behaviour
    [ApiController]


    // sets the base route for this controller
    // URL for all endpoints inside this controller
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllInstructors()
        {
            var data = _context.Instructors.ToList();

            return Ok(new ApiResponse<List<Instructor>>
            {
                Success = true,
                Message = "Instructors retrieved successfully",
                Data = data
            });
        }
    }
}