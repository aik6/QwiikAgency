using Microsoft.AspNetCore.Mvc;
using QwiikAgency.Controller;
using QwiikAgency.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using QwiikAgency.ModelAdd;

namespace UnitTest.CustomerTest
{
    public class AddCustomerTest
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
        public async Task AddCustomer_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);
            controller.ModelState.AddModelError("CustName", "Customer name is required.");

            var data = new CustomerAdd();

            var result = await controller.AddCustomer(data);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddCustomer_ReturnsCreatedAtAction_WhenCustomerIsAddedSuccessfully()
        {
            using var context = CreateContext();
            var controller = new CustomerController(context);

            var data = new CustomerAdd
            {
                CustName = "Test Customer",
                CustAddress = "123 Test St",
                CustPhone = "123-456-7890"
            };

            var result = await controller.AddCustomer(data);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CustomerController.GetCustomerById), createdAtActionResult.ActionName);
            Assert.Equal("Customer added successfully.", createdAtActionResult.Value);

            var addedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.CustName == "Test Customer");
            Assert.NotNull(addedCustomer);
        }
    }
}
