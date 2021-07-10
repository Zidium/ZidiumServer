namespace Zidium.Core.Common
{
    internal interface ILogicSettingsService
    {
        string WebSite();

        string MasterPassword();

        int EventsMaxDays();

        int UnitTestsMaxDays();

        int MetricsMaxDays();

        int LogMaxDays();
    }
}
