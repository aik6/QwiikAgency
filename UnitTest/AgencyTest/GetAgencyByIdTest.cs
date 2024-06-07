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
    public class GetAgencyByIdTest
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
        public async Task GetAgencyById_ReturnsNotFound_WhenAgencyDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);

            var result = await controller.GetAgencyById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAgencyById_ReturnsOk_WhenAgencyExists()
        {
            using var context = CreateContext();
            context.Agencies.Add(new Agency
            {
                AgencyId = 1,
                AgencyName = "Test Agency",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890",
                AgencyMaxAppointment = 10
            });
            await context.SaveChangesAsync();

            var controller = new AgencyController(context);

            var result = await controller.GetAgencyById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var agency = Assert.IsType<Agency>(okResult.Value);
            Assert.Equal("Test Agency", agency.AgencyName);
        }
    }
}
