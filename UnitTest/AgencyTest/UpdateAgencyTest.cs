using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace UnitTest.AgencyTest
{
    public class UpdateAgencyTest
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
        public async Task UpdateAgency_ReturnsNotFound_WhenAgencyDoesNotExist()
        {
            using var context = CreateContext();
            var data = new Agency
            {
                AgencyId = 1,
                AgencyName = "Test Agency",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890",
                AgencyMaxAppointment = 10
            };

            var controller = new AgencyController(context);

            var result = await controller.UpdateAgency(data);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAgency_ReturnsOk_WhenAgencyIsUpdatedSuccessfully()
        {
            using var context = CreateContext();
            context.Agencies.Add(new Agency
            {
                AgencyId = 1,
                AgencyName = "Test Agency",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890",
                AgencyMaxAppointment = 5
            });
            await context.SaveChangesAsync();

            var data = new Agency
            {
                AgencyId = 1,
                AgencyName = "Updated Agency",
                AgencyAddress = "456 Updated St",
                AgencyPhone = "987-654-3210",
                AgencyMaxAppointment = 15
            };

            var controller = new AgencyController(context);

            var result = await controller.UpdateAgency(data);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Agency updated successfully.", okResult.Value);

            var updatedAgency = await context.Agencies.FindAsync(1);
            Assert.NotNull(updatedAgency);
            Assert.Equal(data.AgencyName, updatedAgency.AgencyName);
            Assert.Equal(data.AgencyAddress, updatedAgency.AgencyAddress);
            Assert.Equal(data.AgencyPhone, updatedAgency.AgencyPhone);
            Assert.Equal(data.AgencyMaxAppointment, updatedAgency.AgencyMaxAppointment);
        }
    }
}
