using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class UnitTestsListItemLevel2Model
    {
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        public ComponentForRead Component { get; set; }

        public string DisplayName { get; set; }

        public string Message { get; set; }

        public MonitoringStatus Result { get; set; }
    }
}