using System.Collections.Generic;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class UnitTestsListItemLevel1Model
    {
        public UnitTestType UnitTestType { get; set; }

        public List<UnitTestsListItemLevel2Model> UnitTests { get; set; }
    }
}