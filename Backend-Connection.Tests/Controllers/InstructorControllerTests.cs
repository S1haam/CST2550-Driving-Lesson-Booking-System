using Backend_Connection.Controllers;          
using Backend_Connection.Models;               
using Backend_Connection.Data;                 
using Microsoft.AspNetCore.Mvc;                
using Microsoft.VisualStudio.TestTools.UnitTesting; 

namespace Backend_Connection.Tests.Controllers
{
    // Marks this class as a test class for MSTest
    [TestClass]
    public class InstructorControllerTests
    {
        // Marks this method as a test method
        [TestMethod]
        public void GetAllInstructors_ReturnsApiResponse()
        {
     
            // creates an in memory database to use for testing with MSTest

            var context = DbContextHelper.CreateDb("InstructorTestDb");

            // add a fake instructor into the in-memory database
            context.Instructors.Add(new Instructor
            {
                InstructorId = 1,
                InstructorCode = "INST001",
                InstructorName = "Test Instructor",
                InstructorEmail = "test@instructor.com",
                InstructorPhone = "0123456789",
                InstructorPasswordHash = "hashedpassword",
                InstructorCarType = "Manual",
                InstructorStatus = "Active"
            });

            // stores the data into the in-memory 
            context.SaveChanges();

            // create the controller and inject the in-memory context
            var controller = new InstructorController(context);

            // Call the GET endpoint
            var result = controller.GetAllInstructors() as OkObjectResult;

        
            // returns a valid http response
            Assert.IsNotNull(result);

  
            // get response from the ApiResponse file
            var response = result.Value as ApiResponse<List<Instructor>>;

            // assert that the response is not null
            Assert.IsNotNull(response);

            // assert that the success flag is true
            Assert.IsTrue(response.Success);

            // assert that the message matches what the controller returns
            Assert.AreEqual("Instructors retrieved successfully", response.Message);

            // assert that exactly one instructor was returned
            Assert.AreEqual(1, response.Data.Count);
        }
    }
}