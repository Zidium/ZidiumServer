using Zidium.Storage;

namespace Zidium.UserAccount.Models.MetricTypes
{
    public class ListModel
    {
        public string Search { get; set; }

        public MetricTypeForRead[] Items { get; set; }
    }
}