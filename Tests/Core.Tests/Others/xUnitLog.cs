using System;
using Xunit.Abstractions;
using Zidium.Api;

namespace Zidium.Tests
{
    public class xUnitLog
    {
        public xUnitLog(ITestOutputHelper output)
        {
            Output = output;
        }

        protected ITestOutputHelper Output;

        public void OnAddLogMessage(IComponentControl componentControl, LogMessage logMessage)
        {
            Output.WriteLine("{0} {1} {2}", logMessage.Date ?? DateTime.Now, logMessage.Level, logMessage.Message);
        }
    }
}
