using System;

namespace Zidium.Core.Common.Helpers
{
    public static class ReportPeriodHelper
    {
        public static ReportPeriodRange GetRange(ReportPeriod period, DateTime to)
        {
            var data = new ReportPeriodRange()
            {
                Period = period,
                To = to
            };
            if (period == ReportPeriod.Hour)
            {
                data.From = data.To.AddHours(-1);
            }
            else if (period == ReportPeriod.Day)
            {
                data.From = data.To.AddDays(-1);
            }
            else if (period == ReportPeriod.Week)
            {
                data.From = data.To.AddDays(-7);
            }
            else if (period == ReportPeriod.Month)
            {
                data.From = data.To.AddMonths(-1);
            }
            else
            {
                throw new Exception("Неизвестное значение period: " + period);
            }
            return data;
        }

        public static ReportPeriodRange RoundToLeft(ReportPeriodRange range, TimeSpan round)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }
            var fromYear = new DateTime(range.From.Year, 1, 1);
            long fromMs = (long)(range.From - fromYear).TotalMilliseconds;
            long roundMs = (long)round.TotalMilliseconds;
            long times = (fromMs / roundMs);
            var from = fromYear.AddMilliseconds(roundMs * times);
            return new ReportPeriodRange()
            {
                Period = range.Period,
                From = from,
                To = from.AddMilliseconds(roundMs * times)
            };
        }

        public static ReportPeriodRange RoundToRight(ReportPeriodRange range, TimeSpan round)
        {
            var newRange = RoundToLeft(range, round);
            if (newRange.From == range.From)
            {
                return range;
            }
            return new ReportPeriodRange()
            {
                Period = range.Period,
                From = range.From + round,
                To = range.To + round
            };
        }
    }
}
