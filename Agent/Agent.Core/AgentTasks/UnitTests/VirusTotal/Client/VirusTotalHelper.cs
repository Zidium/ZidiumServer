using System;
using System.Globalization;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public static class VirusTotalHelper
    {
        public static DateTime ParseDateTime(string text)
        {
            // пример строки: 2020-02-03 10:18:57
            DateTime date = DateTime.ParseExact(
                text,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal); // время в UTC

            return new DateTime(date.Ticks, DateTimeKind.Utc);
        }
    }
}
