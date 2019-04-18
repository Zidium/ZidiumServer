using System;

namespace Zidium.Core.Common.Helpers
{
    public static class UnitTestHelper
    {
        public static string GetDynamicSystemName(Guid unitTestId)
        {
            return "UnitTest_" + unitTestId;
        }

        public static TimeSpan GetDefaultActualTime()
        {
            return TimeSpan.FromMinutes(5); // по умолчанию 5 мин
        }        
    }
}
