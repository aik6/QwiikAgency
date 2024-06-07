using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QwiikAgency.Models;
using System.Globalization;

namespace QwiikAgency.Controller
{    
    [ApiController]
    public class AppointmentController : ControllerBase
    { 
        private readonly QwiikAgencyContext _context;

        public AppointmentController(QwiikAgencyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets Today Agency appointments for the specified agency.
        /// </summary>
        /// <param name="Id">Agency ID</param>
        [HttpGet]
        [Route("api/[controller]/GetTodayAppointment/{Id}")]
        public async Task<IActionResult> GetTodayAppointment(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest("Id parameter is required.");
                }

                var existingAgency = await _context.Agencies.FindAsync(Id);
                if (existingAgency == null)
                {
                    return NotFound("Agency not found.");
                }

                var today = DateTime.Today;
                var appointments = await _context.Appointments
                    .Where(a => a.AgencyId == Id && a.AppointmentDate.Date == today.Date)
                    .ToListAsync();

                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound("No appointments found for today.");
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets Agency appointments for the specified date.
        /// </summary>
        /// <param name="Id">Agency ID</param>
        /// <param name="AppointmentDate">The date for which appointments are requested (format: yyyy-MM-dd).</param>
        [HttpGet]
        [Route("api/[controller]/GetAppointmentByDate/{Id}/{AppointmentDate}")]
        public async Task<IActionResult> GetAppointmentByDate(int Id, string AppointmentDate)
        {
            try
            {
                var existingAgency = await _context.Agencies.FindAsync(Id);
                if (existingAgency == null)
                {
                    return NotFound("Agency not found.");
                }

                if (!DateTime.TryParseExact(AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestedDate))
                {
                    return BadRequest("Invalid date format. Please provide the date in yyyy-MM-dd format.");
                }

                var appointments = await _context.Appointments
                    .Where(a => a.AgencyId == Id && a.AppointmentDate.Date == requestedDate.Date)
                    .ToListAsync();

                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound("No appointments found for the specified date.");
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }       

        /// <summary>
        /// Add Agency appointment.
        /// </summary>
        /// <param name="Id">Agency ID</param>
        /// <param name="Cid">Customer ID</param>
        /// <param name="Adate">Appointments Date (format: yyyy-MM-dd).</param>
        [HttpPost]
        [Route("api/[controller]/AddAppointment/{Id}/{Cid}/{Adate}")]
        public async Task<IActionResult> AddAppointment(int Id, int Cid, string Adate)
        {
            try
            {
                if (!DateTime.TryParse(Adate, out DateTime appointmentDate))
                {
                    return BadRequest("Invalid date format for appointment date.");
                }

                var existingOffDays = await _context.Offdays
                    .FirstOrDefaultAsync(a => a.AgencyId == Id && a.OffDate.Date == appointmentDate.Date);

                if (existingOffDays != null)
                {
                    return Conflict("Date is Holiday for the specified agency and date.");
                }

                var agency = await _context.Agencies.FindAsync(Id);
                if (agency == null)
                {
                    return NotFound("Agency not found.");
                }

                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AgencyId == Id && a.CustId == Cid && a.AppointmentDate.Date == appointmentDate.Date);

                if (existingAppointment != null)
                {
                    return Conflict("Appointment already exists for the specified agency, customer, and date.");
                }

                int maxAppointmentCount = agency.AgencyMaxAppointment;
                int appointmentCount = await _context.Appointments
                    .CountAsync(a => a.AgencyId == Id && a.AppointmentDate.Date == appointmentDate.Date);

                while (appointmentCount >= maxAppointmentCount)
                {
                    appointmentDate = appointmentDate.AddDays(1);

                    bool appointmentExistsNextDay = await _context.Appointments.AnyAsync(a => a.AgencyId == Id && a.CustId == Cid && a.AppointmentDate.Date == appointmentDate.Date);

                    if (appointmentExistsNextDay)
                    {
                        return Conflict("An appointment already on date." + appointmentDate.Date.ToString());
                    }

                    existingOffDays = await _context.Offdays.FirstOrDefaultAsync(a => a.AgencyId == Id && a.OffDate.Date == appointmentDate.Date);

                    if (existingOffDays != null)
                    {
                        continue;
                    }

                    appointmentCount = await _context.Appointments
                        .CountAsync(a => a.AgencyId == Id && a.AppointmentDate.Date == appointmentDate.Date);
                }

                var newAppointment = new Appointment
                {
                    AgencyId = Id,
                    CustId = Cid,
                    AppointmentDate = appointmentDate
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                var appDateArr = appointmentDate.Date.ToString().Split(" ");

                return Ok("Appointment added successfully on date: " + appDateArr[0]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update Agency appointment.
        /// </summary>
        /// <param name="data">Appointment Data model</param>
        [HttpPut]
        [Route("api/[controller]/UpdateAppointment/")]
        public async Task<IActionResult> UpdateAppointment([FromBody] Appointment data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingAppointment = await _context.Appointments.FindAsync(data.AgencyId);
                if (existingAppointment == null)
                {
                    return NotFound("Agency not found.");
                }

                existingAppointment.AppointmentId = data.AppointmentId;
                existingAppointment.AgencyId = data.AgencyId;
                existingAppointment.CustId = data.CustId;
                existingAppointment.AppointmentDate = data.AppointmentDate;

                await _context.SaveChangesAsync();

                return Ok("Appointment updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        /// <summary>
        /// Delete Appointment.
        /// </summary>
        /// <param name="Id">Appointment ID</param>
        [HttpDelete]
        [Route("api/[controller]/DeleteAppointment/{Id}")]
        public async Task<IActionResult> DeleteAppointment(int Id)
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

                return Ok("Appointment deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Please try again later. Error: {ex.Message}");
            }
        }
    }
}
