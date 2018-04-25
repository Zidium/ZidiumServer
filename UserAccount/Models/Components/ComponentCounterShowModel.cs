using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    public class ComponentCounterShowModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public MonitoringStatus Color { get; set; }

        public double? Value { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public MetricType MetricType { get; set; }

        public Component Component { get; set; }

        public string ConditionRed { get; set; }

        public string ConditionYellow { get; set; }

        public string ConditionGreen { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }
    }
}