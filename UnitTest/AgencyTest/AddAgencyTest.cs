using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using QwiikAgency.ModelAdd;

namespace UnitTest.AgencyTest
{
    public class AddAgencyTest
    {
        private QwiikAgencyContext CreateContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<QwiikAgencyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            return new QwiikAgencyContext(options);
        }

        [Fact]
        public async Task AddAgency_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);
            controller.ModelState.AddModelError("AgencyName", "AgencyName is required.");

            var data = new AgencyAdd(); // No properties set, hence invalid ModelState.

            var result = await controller.AddAgency(data);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddAgency_ReturnsCreatedAtAction_WhenAgencyIsAddedSuccessfully()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);

            var data = new AgencyAdd
            {
                AgencyName = "Test Agency",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890"
            };

            var result = await controller.AddAgency(data);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AgencyController.GetAgencyById), createdAtActionResult.ActionName);
            Assert.Equal("Agency added successfully.", createdAtActionResult.Value);

            var addedAgency = await context.Agencies.FirstOrDefaultAsync(a => a.AgencyName == "Test Agency");
            Assert.NotNull(addedAgency);
        }
    }
}
