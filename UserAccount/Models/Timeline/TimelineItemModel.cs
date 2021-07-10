using System;
using System.Globalization;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class TimelineItemModel
    {
        public Guid? EventId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EventImportance? Status { get; set; }

        public string Message { get; set; }

        public double Width { get; set; }

        public string WidthStr
        {
            get
            {
                return Width.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string Color
        {
            get
            {
                if (Status == EventImportance.Alarm)
                    return GuiHelper.StrongRedBgColor;

                if (Status == EventImportance.Warning)
                    return GuiHelper.StrongYellowBgColor;

                if (Status == EventImportance.Success)
                    return GuiHelper.StrongGreenBgColor;

                if (Status == EventImportance.Unknown)
                    return TimelineHelper.UnknownStatusColor;

                return TimelineHelper.NoStatusColor;
            }
        }

        public string StartDateStr
        {
            get { return GuiHelper.GetDateTimeString(StartDate); }
        }

        public string EndDateStr
        {
            get { return GuiHelper.GetDateTimeString(EndDate); }
        }

        public int? Count { get; set; }

        public string CountStr
        {
            get { return NumbersHelper.Amount(Count); }
        }

        public TimeSpan Duration
        {
            get { return EndDate - StartDate; }
        }

        public string DurationStr
        {
            get { return GuiHelper.TimeSpanAs2UnitString(Duration); }
        }
    }
}