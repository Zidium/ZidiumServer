using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.MetricTypes
{
    public class ListModel
    {
        public string Search { get; set; }

        public IQueryable<MetricType> Items { get; set; }
    }
}