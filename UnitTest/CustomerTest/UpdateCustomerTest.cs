using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.CustomerTest
{
    public class UpdateCustomerTest
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
        public async Task UpdateCustomer_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);
            controller.ModelState.AddModelError("CustName", "Customer name is required.");

            var data = new Customer(); // No properties set, hence invalid ModelState.

            var result = await controller.UpdateCustomer(data);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);

            var data = new Customer
            {
                CustId = 1,
                CustName = "Test Customer",
                CustAddress = "123 Test St",
                CustPhone = "123-456-7890"
            };

            var result = await controller.UpdateCustomer(data);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsOk_WhenCustomerIsUpdatedSuccessfully()
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

            var data = new Customer
            {
                CustId = 1,
                CustName = "Updated Customer",
                CustAddress = "456 Updated St",
                CustPhone = "987-654-3210"
            };

            var result = await controller.UpdateCustomer(data);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer updated successfully.", okResult.Value);

            var updatedCustomer = await context.Customers.FindAsync(1);
            Assert.NotNull(updatedCustomer);
            Assert.Equal(data.CustName, updatedCustomer.CustName);
            Assert.Equal(data.CustAddress, updatedCustomer.CustAddress);
            Assert.Equal(data.CustPhone, updatedCustomer.CustPhone);
        }
    }
}
