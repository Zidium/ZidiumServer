using System;
using System.Collections.Generic;

namespace Zidium.Core.Common.Helpers
{
    /// <summary>
    /// Результат самопроверки компонента
    /// </summary>
    public class ComponentSelfTestResult
    {
        public Dictionary<string, Exception> UnitTests { get; set; }

        public bool Success { get; set; }

        public string Log { get; set; }
    }
}
