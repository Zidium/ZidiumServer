using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.UserAccount.Models
{
    public class ComponentStatusControlModel
    {
        public Guid ComponentId { get; set; }
        public Bulb State { get; set; }
        public string TargetId { get; set; }
    }
}