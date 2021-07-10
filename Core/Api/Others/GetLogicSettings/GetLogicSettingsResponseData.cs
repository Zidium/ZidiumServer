namespace Zidium.Core.Api
{
    public class GetLogicSettingsResponseData
    {
        public string AccountWebSite { get; set; }

        public int EventsMaxDays { get; set; }

        public int LogMaxDays { get; set; }

        public int MetricsMaxDays { get; set; }

        public int UnitTestsMaxDays { get; set; }
    }
}
