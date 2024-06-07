using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.CustomerTest
{
    public class GetCustomerByIdTest
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
        public async Task GetCustomerById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);

            var result = await controller.GetCustomerById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsOk_WhenCustomerExists()
        {
            using var context = CreateContext();
            context.Customers.Add(new Customer
            {
                CustId = 1,
                CustName = "Test Customer",
                CustAddress = "123 Test St",
                CustPhone = "123-456-7890"
            });
            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            var result = await controller.GetCustomerById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var customer = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal(1, customer.CustId);
            Assert.Equal("Test Customer", customer.CustName);
            Assert.Equal("123 Test St", customer.CustAddress);
            Assert.Equal("123-456-7890", customer.CustPhone);
        }
    }
}
