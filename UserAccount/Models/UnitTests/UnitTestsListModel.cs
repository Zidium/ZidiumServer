using System;
using System.Collections.Generic;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UnitTestsListModel
    {
        public Guid AccountId { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public Guid? ComponentId { get; set; }

        public Guid? UnitTestTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public List<UnitTestsListItemLevel1Model> UnitTestTypes { get; set; }
    }
}