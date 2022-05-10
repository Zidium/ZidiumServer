using System;
using System.Collections.Generic;
using System.Threading;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalLimitManager
    {
        private Dictionary<string, LimitData> lastApiCallTime = new Dictionary<string, LimitData>();

        public void SleepByLimits(string apikey)
        {
            LimitData data = GetLimitData(apikey);
            data.Sleep();
        }

        private LimitData GetLimitData(string apiKey)
        {
            lock (this)
            {
                if (lastApiCallTime.TryGetValue(apiKey, out LimitData data))
                {
                    return data;
                }
                data = new LimitData();
                lastApiCallTime.Add(apiKey, data);
                return data;
            }
        }

        private class LimitData
        {
            private DateTime? lastTime;

            public void Sleep()
            {
                lock (this)
                {
                    if (lastTime == null)
                    {
                        lastTime = DateTime.Now;
                        return;
                    }
                    // The Public API is limited to 4 requests per minute
                    // Перестрахуемся, будем делать вызовы раз в 30 сек (2 запроса в минуту)
                    var sleepDuration = TimeSpan.FromSeconds(30);
                    // TOD Refactor to StopWatch
                    var duration = DateTime.Now - lastTime.Value;
                    if (duration < sleepDuration)
                    {
                        var sleepTime = sleepDuration - duration;
                        Thread.Sleep(sleepTime); // нужно засыпать по 1 сек и проверять cancelation
                    }
                    lastTime = DateTime.Now;
                }
            }
        }
    }
}
