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
    public class SetOffDayTest
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
        public async Task SetOffDay_ReturnsNotFound_WhenAgencyDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new AgencyController(context);

            var result = await controller.SetOffDay(1, "2024-06-10");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SetOffDay_ReturnsBadRequest_WhenDateFormatIsInvalid()
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

            var result = await controller.SetOffDay(1, "10-06-2024");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SetOffDay_ReturnsConflict_WhenOffDayAlreadyExists()
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
            context.Offdays.Add(new Offday
            {
                AgencyId = 1,
                OffDate = DateTime.ParseExact("2024-06-10", "yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
            await context.SaveChangesAsync();

            var controller = new AgencyController(context);

            var result = await controller.SetOffDay(1, "2024-06-10");

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task SetOffDay_ReturnsOk_WhenOffDayIsSetSuccessfully()
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

            var result = await controller.SetOffDay(1, "2024-06-10");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Offday added successfully on date: 10/06/2024", okResult.Value);

            var offDayInDb = await context.Offdays.FirstOrDefaultAsync(o => o.AgencyId == 1 && o.OffDate == DateTime.ParseExact("2024-06-10", "yyyy-MM-dd", CultureInfo.InvariantCulture));
            Assert.NotNull(offDayInDb);
        }
    }
}
