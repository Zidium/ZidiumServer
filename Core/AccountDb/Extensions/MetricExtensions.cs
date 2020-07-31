using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class MetricExtensions
    {
        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        public static bool CanProcess(this MetricForRead metric)
        {
            return metric.Enable && metric.ParentEnable;
        }
    }
}
