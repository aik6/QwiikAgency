using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QwiikAgency.ModelAdd;
using QwiikAgency.Models;

namespace QwiikAgency.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly QwiikAgencyContext _context;
        public CustomerController(QwiikAgencyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets All Customer
        /// </summary>
        [HttpGet]
        [Route("api/[controller]/GetCustomer/")]
        public async Task<IActionResult> GetCustomer()
        {
            try
            {
                var customers = await _context.Customers
                    .ToListAsync();

                if (customers == null || customers.Count == 0)
                {
                    return NotFound("No Customer found.");
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets Customer by ID
        /// </summary>
        /// <param name="Id">Customer ID</param>
        [HttpGet]
        [Route("api/[controller]/GetCustomerById/{Id}")]
        public async Task<IActionResult> GetCustomerById(int Id)
        {
            try
            {
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(a => a.CustId == Id);

                if (existingCustomer == null)
                {
                    return NotFound("Customer not found.");
                }

                return Ok(existingCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Customer Add
        /// </summary>
        /// <param name="data">Customer Data model</param>
        [HttpPost]
        [Route("api/[controller]/AddCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerAdd data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newcustomer = new Customer
                {
                    CustName = data.CustName,
                    CustAddress = data.CustAddress,
                    CustPhone = data.CustPhone
                };

                _context.Customers.Add(newcustomer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomerById), new { Id = newcustomer.CustId }, "Customer added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Customer Update
        /// </summary>
        /// <param name="data">Customer Data model</param>
        [HttpPut]
        [Route("api/[controller]/UpdateCustomer/")]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCustomer = await _context.Customers.FindAsync(data.CustId);
                if (existingCustomer == null)
                {
                    return NotFound("Customer not found.");
                }

                existingCustomer.CustName = data.CustName;
                existingCustomer.CustAddress = data.CustAddress;
                existingCustomer.CustPhone = data.CustPhone;

                await _context.SaveChangesAsync();

                return Ok("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Customer Delete
        /// </summary>
        /// <param name="Id">Customer ID</param>
        [HttpDelete]
        [Route("api/[controller]/DeleteCustomer/{Id}")]
        public async Task<IActionResult> DeleteCustomer(int Id)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(Id);
                if (existingCustomer == null)
                {
                    return NotFound("Customer not found.");
                }

                _context.Customers.Remove(existingCustomer);
                await _context.SaveChangesAsync();

                return Ok("Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }
    }
}
