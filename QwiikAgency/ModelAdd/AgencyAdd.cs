namespace QwiikAgency.ModelAdd
{
    public class AgencyAdd
    {
        public string AgencyName { get; set; } = null!;

        public string? AgencyAddress { get; set; }

        public string? AgencyPhone { get; set; }

        public int AgencyMaxAppointment { get; set; }
    }
}
