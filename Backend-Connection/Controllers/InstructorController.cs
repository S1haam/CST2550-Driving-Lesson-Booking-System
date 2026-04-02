using Backend_Connection.Data;
using Backend_Connection.Models;
using Microsoft.AspNetCore.Mvc;


namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context; //saved to be used by other methods
        }


        //GET api/instructor
        [HttpGet]
        public IActionResult GetAllInstructors()
        {
            //fetches all data from the Instructors table in the SQL server
            var instructors = _context.Instructors.ToList();


            //data is given in the ApiResponse format
            //Used for consistency for other API responses
            return Ok(new ApiResponse<List<Instructor>>(
                true,
                "Instructors retrieved successfully",
                instructors //actual data passed onto here
            ));
        }
    }
}