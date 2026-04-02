using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Connection.Controllers
{


    // API controller being used
    //Used for automatic validation and cleaerer error responses
    [ApiController]


    //Route for the controller
    //[Controller] changes to the controller being used
    //eg [Availabilty] would be api/[Availability]

    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        //Private variable to store the databse context
        //Talks to the SQL server database
        private readonly ApplicationDbContext _context;


        //Constructor Injection
        //ASP.NET.CORE automtically provides the instance being used
        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context; //context saved to be used in the methods
        }

        // GET: api/availability
        [HttpGet]
        public IActionResult GetAllAvailability()
        {

            //fetch all the rows for the table stores in the SQL server
            var data = _context.Availabilities.ToList();

            //data is being returned in the form of a JSON
            return Ok(data);
        }
    }
}