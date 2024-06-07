using System;
using System.Collections.Generic;

namespace QwiikAgency.Models;

public partial class Agency
{
    public int AgencyId { get; set; }

    public string AgencyName { get; set; } = null!;

    public string? AgencyAddress { get; set; }

    public string? AgencyPhone { get; set; }

    public int AgencyMaxAppointment { get; set; }
}
