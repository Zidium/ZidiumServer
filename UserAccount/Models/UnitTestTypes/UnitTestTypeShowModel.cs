using System;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    public class UnitTestTypeShowModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public bool IsSystem { get; set; }

        public TimeSpan? ActualTime { get; set; }

        public TimeSpan ActualTimeDefault { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public ObjectColor NoSignalColorDefault { get; set; }

        public bool IsDeleted { get; set; }
    }
}