using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class UnitTestTypeListModel
    {
        public string Search { get; set; }

        public IQueryable<UnitTestType> Items { get; set; }
    }
}