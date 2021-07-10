namespace Zidium.UserAccount.Models
{
    public class ComponentErrorStatisticsModel
    {
        public long ByHour { get; set; }

        public long ByDay { get; set; }

        public long ByWeek { get; set; }

        public long ByMonth { get; set; }

        public bool ShowWait { get; set; }
    }
}