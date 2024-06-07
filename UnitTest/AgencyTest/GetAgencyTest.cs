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
    public class GetAgencyTest
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
        public async Task GetAgency_ReturnsNotFound_WhenNoAgenciesExist()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);

            var result = await controller.GetAgency();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAgency_ReturnsOk_WhenAgenciesExist()
        {
            using var context = CreateContext();
            context.Agencies.Add(new Agency
            {
                AgencyId = 1,
                AgencyName = "Test Agency 1",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890",
                AgencyMaxAppointment = 10
            });
            context.Agencies.Add(new Agency
            {
                AgencyId = 2,
                AgencyName = "Test Agency 2",
                AgencyAddress = "456 Test St",
                AgencyPhone = "987-654-3210",
                AgencyMaxAppointment = 20
            });
            await context.SaveChangesAsync();

            var controller = new AgencyController(context);

            var result = await controller.GetAgency();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var agencies = Assert.IsAssignableFrom<IEnumerable<Agency>>(okResult.Value);
            Assert.Equal(2, agencies.Count());
            Assert.Contains(agencies, a => a.AgencyName == "Test Agency 1");
            Assert.Contains(agencies, a => a.AgencyName == "Test Agency 2");
        }
    }
}
