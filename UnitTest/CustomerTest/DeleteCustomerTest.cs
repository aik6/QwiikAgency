using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.CustomerTest
{
    public class DeleteCustomerTest
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
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);

            var result = await controller.DeleteCustomer(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsOk_WhenCustomerIsDeletedSuccessfully()
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

            var result = await controller.DeleteCustomer(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer deleted successfully.", okResult.Value);

            var deletedCustomer = await context.Customers.FindAsync(1);
            Assert.Null(deletedCustomer);
        }
    }
}
