using System;
using System.Linq;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models.MetricData
{
    public class MetricGraphDataModel
    {
        public string Name { get; set; }

        public MetricGraphDataItem[] GraphData { get; set; }

        public string JsGraphData
        {
            get
            {
                return string.Join(", ", GraphData.Select(t =>
                    $"{{x:{GuiHelper.DateTimeToHighChartsFormat(t.Date)},y:{(t.Value.HasValue ? t.Value.ToString() : "null")}, color:'{GuiHelper.GetStrongFgColor(t.Color)}'}}"
                    ));
            }
        }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public double? Avg { get; set; }

        public double? AvgAsString
        {
            get
            {
                if (Min == null && Max == null)
                    return null;

                if (Avg == null)
                    return null;

                var decimalCountInMin = Min.HasValue ? GuiHelper.GetDecimalPlaces((decimal)Min.Value) : 0;
                var decimalCountInMax = Max.HasValue ? GuiHelper.GetDecimalPlaces((decimal)Max.Value) : 0;
                var decimalPlaces = Math.Max(decimalCountInMin, decimalCountInMax);

                return Math.Round(Avg.Value, decimalPlaces);
            }
        }
    }
}