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
    public class SetMaxAppointmentTest
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
        public async Task SetMaxAppointment_ReturnsNotFound_WhenAgencyDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);

            var result = await controller.SetMaxAppointment(1, 10);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SetMaxAppointment_ReturnsOk_WhenMaxAppointmentIsUpdatedSuccessfully()
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

            var controller = new AgencyController(context);

            var result = await controller.SetMaxAppointment(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Agency Max appointment updated successfully.", okResult.Value);

            var updatedAgency = await context.Agencies.FindAsync(1);
            Assert.NotNull(updatedAgency);
            Assert.Equal(10, updatedAgency.AgencyMaxAppointment);
        }
    }
}
