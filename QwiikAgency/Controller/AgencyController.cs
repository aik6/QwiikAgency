using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QwiikAgency.ModelAdd;
using QwiikAgency.Models;
using System.Globalization;
using System.Security.Cryptography;

namespace QwiikAgency.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly QwiikAgencyContext _context;
        public AgencyController(QwiikAgencyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets All Agency
        /// </summary>
        [HttpGet]
        [Route("api/[controller]/GetAgency/")]
        public async Task<IActionResult> GetAgency()
        {
            try
            {
                var agency = await _context.Agencies
                    .ToListAsync();

                if (agency == null || agency.Count == 0)
                {
                    return NotFound("No Agency found.");
                }

                return Ok(agency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets Agency by ID
        /// </summary>
        /// <param name="Id">Agency ID</param>
        [HttpGet]
        [Route("api/[controller]/GetAgencyById/{Id}")]
        public async Task<IActionResult> GetAgencyById(int Id)
        {
            try
            {
                var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.AgencyId == Id);

                if (agency == null)
                {
                    return NotFound("Agency not found.");
                }

                return Ok(agency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Agency Add
        /// </summary>
        /// <param name="data">Agency Data model</param>
        [HttpPost]
        [Route("api/[controller]/AddAgency")]
        public async Task<IActionResult> AddAgency([FromBody] AgencyAdd data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newagency = new Agency
                {
                    AgencyName = data.AgencyName,
                    AgencyAddress = data.AgencyAddress,
                    AgencyPhone = data.AgencyPhone
                };

                _context.Agencies.Add(newagency);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAgencyById), new { Id = newagency.AgencyId }, "Agency added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Agency Update
        /// </summary>
        /// <param name="data">Agency Data model</param>
        [HttpPut]
        [Route("api/[controller]/UpdateAgency/")]
        public async Task<IActionResult> UpdateAgency([FromBody] Agency data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingAgency = await _context.Agencies.FindAsync(data.AgencyId);
                if (existingAgency == null)
                {
                    return NotFound("Agency not found.");
                }

                existingAgency.AgencyName = data.AgencyName;
                existingAgency.AgencyAddress = data.AgencyAddress;
                existingAgency.AgencyPhone = data.AgencyPhone;
                existingAgency.AgencyMaxAppointment = data.AgencyMaxAppointment;

                await _context.SaveChangesAsync();

                return Ok("Agency updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets Agency max appointments per day.
        /// </summary>
        /// <param name="Id">Agency ID</param>
        /// <param name="MaxValue">Appointments max Value</param>
        [HttpPut]
        [Route("api/[controller]/SetMaxAppointment/{Id}")]
        public async Task<IActionResult> SetMaxAppointment(int Id, int MaxValue)
        {
            try
            {
                var agency = await _context.Agencies.FindAsync(Id);
                if (agency == null)
                {
                    return NotFound("Agency not found.");
                }

                agency.AgencyMaxAppointment = MaxValue;
                await _context.SaveChangesAsync();

                return Ok("Agency Max appointment updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Agency Set Offday
        /// </summary>
        /// <param name="Id">Agency ID</param>
        /// <param name="OffDate">Offday Date (format: yyyy-MM-dd).</param>
        [HttpPost]
        [Route("api/[controller]/SetOffDay/{Id}/{OffDate}")]
        public async Task<IActionResult> SetOffDay(int Id, string OffDate)
        {
            try
            {
                var existingAgency = await _context.Agencies.FindAsync(Id);
                if (existingAgency == null)
                {
                    return NotFound("Agency not found.");
                }

                if (!DateTime.TryParseExact(OffDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestedDate))
                {
                    return BadRequest("Invalid date format. Please provide the date in yyyy-MM-dd format.");
                }

                var existingOffday = await _context.Offdays.FirstOrDefaultAsync(a => a.AgencyId == Id & a.OffDate.Date == requestedDate.Date);

                if (existingOffday != null)
                {
                    return Conflict("Off day already exists for the specified agency and date.");
                }

                var newOffday = new Offday
                {
                    AgencyId = Id,
                    OffDate = requestedDate
                };

                _context.Offdays.Add(newOffday);
                await _context.SaveChangesAsync();

                var offDayDateArr = requestedDate.Date.ToString().Split(" ");

                return Ok("Offday added successfully on date: " + offDayDateArr[0]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Agency Delete
        /// </summary>
        /// <param name="Id">Agency ID</param>
        [HttpDelete]
        [Route("api/[controller]/DeleteAgency/{Id}")]
        public async Task<IActionResult> DeleteAgency(int Id)
        {
            try
            {
                var existingAgency = await _context.Agencies.FindAsync(Id);
                if (existingAgency == null)
                {
                    return NotFound("Agency not found.");
                }

                _context.Agencies.Remove(existingAgency);
                await _context.SaveChangesAsync();

                return Ok("Agency deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }
    }
}
