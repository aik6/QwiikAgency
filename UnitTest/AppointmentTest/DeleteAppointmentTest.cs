using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.AppointmentTest
{
    public class DeleteAppointmentTest
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
        public async Task DeleteAppointment_ReturnsNotFound_WhenAgencyDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new AppointmentController(context);

            var result = await controller.DeleteAppointment(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsOk_WhenAppointmentDeletedSuccessfully()
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

            var controller = new AppointmentController(context);

            var result = await controller.DeleteAppointment(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Appointment deleted successfully.", okResult.Value);

            var agencyInDb = await context.Agencies.FindAsync(1);
            Assert.Null(agencyInDb);
        }
    }
}
