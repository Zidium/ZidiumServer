using System;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models.Controls
{
    public class MonitoringStatusModel
    {
        public MonitoringStatus Status { get; set; }

        public Guid? StatusEventId { get; set; }

        public int FontSize { get; set; }

        public string Text { get; set; }

        public string GetCssClass()
        {
            return GuiHelper.GetMonitoringStatusLabelCssClass(Status);
        }
    }
}