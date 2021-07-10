namespace Zidium.Core
{
    public interface ILogicConfiguration
    {
        string WebSite { get; }

        string MasterPassword { get; }

        int? EventsMaxDays { get; }

        int? LogMaxDays { get; }

        int? MetricsMaxDays { get; }

        int? UnitTestsMaxDays { get; }

    }
}
