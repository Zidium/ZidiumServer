using System;
using System.Collections.Generic;
using System.Threading;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalLimitManager
    {
        private Dictionary<string, DateTime> lastApiCallTime = new Dictionary<string, DateTime>();

        public void SleepByLimits(string apikey)
        {
            if (lastApiCallTime.ContainsKey(apikey) == false)
            {
                // первый вызов
                lastApiCallTime.Add(apikey, DateTime.Now);
                return;
            }

            // The Public API is limited to 4 requests per minute
            // Перестрахуемся, будем делать вызовы раз в 20 сек (3 запроса в минуту)
            var sleepDuration = TimeSpan.FromSeconds(20);
            DateTime lastTime = lastApiCallTime[apikey];
            var duration = DateTime.Now - lastTime;
            if (duration < sleepDuration)
            {
                var sleepTime = sleepDuration - duration;
                Thread.Sleep(sleepTime); // нужно засыпать по 1 сек и проверять cancelation
            }
            lastApiCallTime[apikey] = DateTime.Now;
        }
    }
}
