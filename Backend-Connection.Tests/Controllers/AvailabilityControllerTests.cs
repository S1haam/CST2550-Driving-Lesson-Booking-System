using Backend_Connection.Controllers;
using Backend_Connection.Models;
using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend_Connection.Tests.Controllers
{
    [TestClass]
    public class AvailabilityControllerTests
    {
        [TestMethod]
        public void GetAllAvailability_ReturnsApiResponse()
        {
            // Arrange
            var context = DbContextHelper.CreateDb("AvailabilityTestDb");

            // Add fake availability
            context.Availabilities.Add(new Availability
            {
                AvailabilityId = 1,
                InstructorId = 1,
                AvailableDateTime = new DateTime(2026, 04, 03, 10, 00, 00),
                IsTaken = false
            });

            context.SaveChanges();

            var controller = new AvailabilityController(context);

            // Act
            var result = controller.GetAllAvailability() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);

            var response = result.Value as ApiResponse<List<Availability>>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Availabilities retrieved successfully", response.Message);
            Assert.AreEqual(1, response.Data.Count);
        }
    }
}