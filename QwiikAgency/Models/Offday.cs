using System;
using System.Collections.Generic;

namespace QwiikAgency.Models;

public partial class Offday
{
    public int Id { get; set; }

    public int AgencyId { get; set; }

    public DateTime OffDate { get; set; }
}
