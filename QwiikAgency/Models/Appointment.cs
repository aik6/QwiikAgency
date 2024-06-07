using System;
using System.Collections.Generic;

namespace QwiikAgency.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int AgencyId { get; set; }

    public int CustId { get; set; }

    public DateTime AppointmentDate { get; set; }
}
