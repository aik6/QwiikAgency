using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.CustomerTest
{
    public class GetCustomerTest
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
        public async Task GetCustomer_ReturnsNotFound_WhenNoCustomersExist()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);

            var result = await controller.GetCustomer();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomer_ReturnsOk_WhenCustomersExist()
        {
            using var context = CreateContext();
            context.Customers.Add(new Customer
            {
                CustId = 1,
                CustName = "Test Customer 1",
                CustAddress = "123 Test St",
                CustPhone = "123-456-7890"
            });
            context.Customers.Add(new Customer
            {
                CustId = 2,
                CustName = "Test Customer 2",
                CustAddress = "456 Test Ave",
                CustPhone = "987-654-3210"
            });
            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            var result = await controller.GetCustomer();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var customers = Assert.IsType<List<Customer>>(okResult.Value);
            Assert.Equal(2, customers.Count);
        }
    }
}
