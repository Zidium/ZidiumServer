using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.UserAccount.Models.Controls
{
	public class OneLineSausageCharModel
    {
		public class IntervalData
		{
		    public int Index { get; set; }

		    public DateTime FromTime { get; set; }

		    public DateTime ToTime { get; set; }

            public string Color { get; set; }

		    public bool IsSpace { get; set; }

		    public int Count { get; set; }
		}

        /// <summary>
        /// В этих интервалах колбаски
        /// </summary>
        protected IntervalData[] RowIntervals { get; set; }

        /// <summary>
        /// Тут интервалы без пробелов
        /// </summary>
        public IntervalData[] ResultIntervals { get; protected set; }

        public string NoDataColor { get; set; }

        public string DataColor { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public double Coverage { get; protected set; }

        public long Count { get; set; }

        public void Init(IntervalData[] intervals, long count)
        {
            if (intervals == null)
            {
                throw new ArgumentNullException("intervals");
            }
            Count = count;
            RowIntervals = intervals;

            RowIntervals = RowIntervals.OrderBy(x => x.FromTime).ToArray();
            var resultIntervals = new List<IntervalData>();
            
            var lastTime = FromTime;
            IntervalData lastResultInterval = null;
            foreach (var intervalData in RowIntervals)
            { 
                if (intervalData.ToTime < FromTime)
                {
                    continue;
                }
                if (intervalData.FromTime > ToTime)
                {
                    continue;
                }
                // проверим есть ли пробел
                if (intervalData.FromTime > lastTime)
                {
                    var space = new IntervalData()
                    {
                        FromTime = lastTime,
                        ToTime = intervalData.FromTime,
                        Color = NoDataColor,
                        IsSpace = true
                    };
                    resultIntervals.Add(space);
                    lastResultInterval = new IntervalData()
                    {
                        FromTime = intervalData.FromTime,
                        ToTime = intervalData.ToTime,
                        Color = DataColor
                    };
                    resultIntervals.Add(lastResultInterval);
                    lastTime = lastResultInterval.ToTime;
                }
                else // пробела нет
                {
                    // первый интервал
                    if (lastResultInterval == null)
                    {
                        lastResultInterval = new IntervalData()
                        {
                            FromTime = intervalData.FromTime,
                            ToTime = intervalData.ToTime,
                            Color = DataColor
                        };
                        resultIntervals.Add(lastResultInterval);
                    }
                    // проверим можно ли увеличить колбаску
                    else if (intervalData.ToTime > lastResultInterval.ToTime)
                    {
                        lastResultInterval.ToTime = intervalData.ToTime;
                    }
                }
                lastTime = lastResultInterval.ToTime;
            }
            if (resultIntervals.Count > 0)
            {
                var last = resultIntervals.Last();
                if (last.ToTime < ToTime)
                {
                    var space = new IntervalData()
                    {
                        FromTime = last.ToTime,
                        ToTime = ToTime,
                        Color = NoDataColor,
                        IsSpace = true
                    };
                    resultIntervals.Add(space);
                }
            }

            if (resultIntervals.Count > 0)
            {
                var first = resultIntervals.First();
                if (first.FromTime < FromTime)
                {
                    first.FromTime = FromTime;
                }
                var last = resultIntervals.Last();
                if (last.ToTime > ToTime)
                {
                    last.ToTime = ToTime;
                }
            }
            
            // статистика
            double coverageSec = 0;
            int index = 0;
            foreach (var resultInterval in resultIntervals)
            {
                if (resultInterval.IsSpace == false)
                {
                    coverageSec += (resultInterval.ToTime - resultInterval.FromTime).TotalSeconds;
                }
                resultInterval.Index = index;
                index++;
            }

            Coverage = coverageSec / ((ToTime - FromTime).TotalSeconds);
            ResultIntervals = resultIntervals.ToArray();
        }

        public string GetIntervalWidth(IntervalData interval)
        {
            double periodSec = (ToTime - FromTime).TotalSeconds;
            double intervalSec = (interval.ToTime - interval.FromTime).TotalSeconds;
            double width = intervalSec / periodSec * 99;
            return Math.Round(width, 4) + "%";
        }
    }
}