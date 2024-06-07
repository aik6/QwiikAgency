using System;
using System.Collections.Generic;

namespace QwiikAgency.Models;

public partial class Customer
{
    public int CustId { get; set; }

    public string CustName { get; set; } = null!;

    public string? CustAddress { get; set; }

    public string? CustPhone { get; set; }
}
