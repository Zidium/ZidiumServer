using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class UnitTestsListItemLevel1Model
    {
        public UnitTestTypeForRead UnitTestType { get; set; }

        public List<UnitTestsListItemLevel2Model> UnitTests { get; set; }
    }
}