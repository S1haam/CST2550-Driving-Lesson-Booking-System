using Backend_Connection.Controllers;
using Backend_Connection.Models;
using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend_Connection.Tests.Controllers
{
    [TestClass]
    public class LearnerControllerTests
    {
        [TestMethod]
        public void GetAllLearners_ReturnsApiResponse()
        {
            // create in-memory DB
            var context = DbContextHelper.CreateDb("LearnerTestDb");

            // add fake learner
            context.Learners.Add(new Learner
            {
                LearnerId = 1,
                LearnerName = "Test Learner",
                LearnerLicenceId = "L1234567",
                LearnerEmail = "test@learner.com",
                LearnerPhone = "07123456789",
                LearnerStatus = "Active",
                PastLessonCount = 0,
                NextLessonCount = 1
            });

            context.SaveChanges();

            // inject into controller
            var controller = new LearnerController(context);

            // Act
            var result = controller.GetAllLearners() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);

            var response = result.Value as ApiResponse<List<Learner>>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Learners retrieved successfully", response.Message);
            Assert.AreEqual(1, response.Data.Count);
        }
    }
}