using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.AppointmentTest
{
    public class GetAppointmentByDateTest
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
        public async Task GetAppointmentByDate_ReturnsNotFound_WhenAgencyNotFound()
        {
            using var context = CreateContext();
            var controller = new AppointmentController(context);
            var result = await controller.GetAppointmentByDate(1, "2024-06-07");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAppointmentByDate_ReturnsBadRequest_WhenDateFormatIsInvalid()
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
            var result = await controller.GetAppointmentByDate(1, "07-06-2024");
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAppointmentByDate_ReturnsNotFound_WhenNoAppointmentsForDate()
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
            var result = await controller.GetAppointmentByDate(1, "2024-06-07");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAppointmentByDate_ReturnsOk_WhenAppointmentsExistForDate()
        {
            var appointmentDate = new DateTime(2024, 6, 7);
            using var context = CreateContext();
            context.Agencies.Add(new Agency
            {
                AgencyId = 1,
                AgencyName = "Test Agency",
                AgencyAddress = "123 Test St",
                AgencyPhone = "123-456-7890",
                AgencyMaxAppointment = 10
            });
            context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                AgencyId = 1,
                CustId = 1,
                AppointmentDate = appointmentDate
            });
            await context.SaveChangesAsync();

            var controller = new AppointmentController(context);
            var result = await controller.GetAppointmentByDate(1, "2024-06-07");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<Appointment>>(okResult.Value);
            Assert.Single(appointments);
        }
    }
}
