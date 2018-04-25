using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Counters
{
    public class ValuesModel
    {
        public Guid? ComponentId { get; set; }

        public Guid? MetricTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public IQueryable<Metric> Items { get; set; }
    }
}