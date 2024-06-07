using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.AppointmentTest
{
    public class UpdateAppointmentTest
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
        public async Task UpdateAppointment_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            using var context = CreateContext();
            var controller = new AppointmentController(context);
            controller.ModelState.AddModelError("AppointmentDate", "Required");

            var appointment = new Appointment
            {
                AppointmentId = 1,
                AgencyId = 1,
                CustId = 1,
                AppointmentDate = DateTime.Now
            };

            var result = await controller.UpdateAppointment(appointment);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new AppointmentController(context);

            var appointment = new Appointment
            {
                AppointmentId = 1,
                AgencyId = 1,
                CustId = 1,
                AppointmentDate = DateTime.Now
            };

            var result = await controller.UpdateAppointment(appointment);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsOk_WhenAppointmentIsUpdatedSuccessfully()
        {
            using var context = CreateContext();
            context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                AgencyId = 1,
                CustId = 1,
                AppointmentDate = new DateTime(2024, 6, 7)
            });
            await context.SaveChangesAsync();

            var controller = new AppointmentController(context);

            var updatedAppointment = new Appointment
            {
                AppointmentId = 1,
                AgencyId = 1,
                CustId = 2,
                AppointmentDate = new DateTime(2024, 6, 8)
            };

            var result = await controller.UpdateAppointment(updatedAppointment);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Appointment updated successfully.", okResult.Value);

            var appointmentInDb = await context.Appointments.FindAsync(1);
            Assert.Equal(2, appointmentInDb.CustId);
            Assert.Equal(new DateTime(2024, 6, 8), appointmentInDb.AppointmentDate);
        }
    }
}
