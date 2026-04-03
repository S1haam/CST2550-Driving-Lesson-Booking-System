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
    public class LearnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LearnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllLearners()
        {
            var data = _context.Learners.ToList();

            return Ok(new ApiResponse<List<Learner>>
            {
                Success = true,
                Message = "Learners retrieved successfully",
                Data = data
            });
        }
    }
}