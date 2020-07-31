using System.Collections.Generic;
using System.Linq;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models.Controls
{
    public class ColorStatusSelectorValue
    {
        public bool RedChecked { get; set; }

        public bool YellowChecked { get; set; }

        public bool GreenChecked { get; set; }

        public bool GrayChecked { get; set; }

        public ColorStatusSelectorValue()
        {
        }

        /// <summary>
        /// Либо все включены, либо все выключены = фильтр не применяется
        /// </summary>
        public bool NotChecked
        {
            get
            {
                if (RedChecked && YellowChecked && GreenChecked && GrayChecked)
                {
                    return true;
                }
                if ((RedChecked == false) && (YellowChecked == false) && (GreenChecked == false) && (GrayChecked == false))
                {
                    return true;
                }
                return false;
            }
        }

        public bool Checked
        {
            get { return NotChecked == false; }
        }

        public bool HasValue
        {
            get { return RedChecked || YellowChecked || GreenChecked || GrayChecked; }
        }

        public ObjectColor? GetSelectedOne()
        {
            var colors = GetSelectedColors();
            if (colors.Length == 0)
                return null;
            return colors.First();
        }

        public ObjectColor[] GetSelectedColors()
        {
            var colors = new List<ObjectColor>();
            if (Checked)
            {
                if (RedChecked)
                {
                    colors.Add(ObjectColor.Red);
                }
                if (YellowChecked)
                {
                    colors.Add(ObjectColor.Yellow);
                }
                if (GreenChecked)
                {
                    colors.Add(ObjectColor.Green);
                }
                if (GrayChecked)
                {
                    colors.Add(ObjectColor.Gray);
                }
            }
            return colors.ToArray();
        }

        public UnitTestResult[] GetSelectedUnitTestResultStatuses()
        {
            var statuses = new List<UnitTestResult>();
            if (Checked)
            {
                if (RedChecked)
                {
                    statuses.Add(UnitTestResult.Alarm);
                }
                if (YellowChecked)
                {
                    statuses.Add(UnitTestResult.Warning);
                }
                if (GreenChecked)
                {
                    statuses.Add(UnitTestResult.Success);
                }
                if (GrayChecked)
                {
                    statuses.Add(UnitTestResult.Unknown);
                }
            }
            return statuses.ToArray();
        }

        public EventImportance[] GetSelectedEventImportances()
        {
            var statuses = new List<EventImportance>();
            if (Checked)
            {
                if (RedChecked)
                {
                    statuses.Add(EventImportance.Alarm);
                }
                if (YellowChecked)
                {
                    statuses.Add(EventImportance.Warning);
                }
                if (GreenChecked)
                {
                    statuses.Add(EventImportance.Success);
                }
                if (GrayChecked)
                {
                    statuses.Add(EventImportance.Unknown);
                }
            }
            return statuses.ToArray();
        }

        public MonitoringStatus[] GetSelectedMonitoringStatuses()
        {
            var statuses = new List<MonitoringStatus>();
            if (Checked)
            {
                if (RedChecked)
                {
                    statuses.Add(MonitoringStatus.Alarm);
                }
                if (YellowChecked)
                {
                    statuses.Add(MonitoringStatus.Warning);
                }
                if (GreenChecked)
                {
                    statuses.Add(MonitoringStatus.Success);
                }
                if (GrayChecked)
                {
                    statuses.Add(MonitoringStatus.Unknown);
                    statuses.Add(MonitoringStatus.Disabled);
                }
            }
            return statuses.ToArray();
        }

        public static ColorStatusSelectorValue FromUnitTestResultStatus(UnitTestResult? status)
        {
            var value = new ColorStatusSelectorValue();
            if (status == null)
            {
                return value;
            }
            if (status.Value == UnitTestResult.Alarm)
            {
                value.RedChecked = true;
                return value;
            }
            if (status.Value == UnitTestResult.Warning)
            {
                value.YellowChecked = true;
                return value;
            }
            if (status.Value == UnitTestResult.Success)
            {
                value.GreenChecked = true;
                return value;
            }
            if (status.Value == UnitTestResult.Unknown)
            {
                value.GrayChecked = true;
                return value;
            }
            return value;
        }

        public static ColorStatusSelectorValue FromEventImportance(EventImportance? importance)
        {
            var value = new ColorStatusSelectorValue();
            if (importance == null)
            {
                return value;
            }
            if (importance.Value == EventImportance.Alarm)
            {
                value.RedChecked = true;
                return value;
            }
            if (importance.Value == EventImportance.Warning)
            {
                value.YellowChecked = true;
                return value;
            }
            if (importance.Value == EventImportance.Success)
            {
                value.GreenChecked = true;
                return value;
            }
            if (importance.Value == EventImportance.Unknown)
            {
                value.GrayChecked = true;
                return value;
            }
            return value;
        }

        public static ColorStatusSelectorValue FromColor(ObjectColor? color)
        {
            var value = new ColorStatusSelectorValue();
            if (color == null)
            {
                return value;
            }
            if (color.Value == ObjectColor.Red)
            {
                value.RedChecked = true;
                return value;
            }
            if (color.Value == ObjectColor.Yellow)
            {
                value.YellowChecked = true;
                return value;
            }
            if (color.Value == ObjectColor.Green)
            {
                value.GreenChecked = true;
                return value;
            }
            if (color.Value == ObjectColor.Gray)
            {
                value.GrayChecked = true;
                return value;
            }
            return value;
        }

        public static ColorStatusSelectorValue FromString(string s)
        {
            var color = EnumHelper.StringToEnum<ObjectColor>(s);
            return FromColor(color);
        }

        public override string ToString()
        {
            return string.Join("~", GetSelectedColors());
        }
    }
}