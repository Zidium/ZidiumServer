using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class UnitTestBreadCrumbsModel
    {
        public ComponentBreadCrumbsModel ComponentBreadCrumbs;

        public string DisplayName;

        public Guid Id;

        public static UnitTestBreadCrumbsModel Create(Guid id, IStorage storage)
        {
            var unittest = storage.UnitTests.GetOneById(id);
            return new UnitTestBreadCrumbsModel()
            {
                Id = id,
                DisplayName = unittest.DisplayName,
                ComponentBreadCrumbs = ComponentBreadCrumbsModel.Create(unittest.ComponentId, storage)
            };
        }
    }
}