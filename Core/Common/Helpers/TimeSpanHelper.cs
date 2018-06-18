using System;

namespace Zidium.Core.Common.Helpers
{
    public static class TimeSpanHelper
    {
        public static TimeSpan? FromSeconds(int? seconds)
        {
            if (seconds == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(seconds.Value);
        }

        public static TimeSpan? FromSeconds(double? seconds)
        {
            if (seconds == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(seconds.Value);
        }

        public static int? GetSeconds(TimeSpan? time)
        {
            if (time == null)
            {
                return null;
            }
            return (int)time.Value.TotalSeconds;
        }

        public static string GetDaysString(int days)
        {
            if (days == 12 || days == 13 || days == 14)
            {
                return days + " дней";
            }
            if (days%10 >= 2 && days%10 <= 4)
            {
                return days + " дня";
            }
            if (days%10 == 1)
            {
                return days + " день";
            }
            return days + " дней";
        }

        public static string GetHoursString(int hours)
        {
            if (hours == 2 || hours == 3 || hours == 4 || hours == 22 || hours == 23)
            {
                return hours + " часа";
            }
            if (hours == 1 || hours == 21)
            {
                return hours + " час";
            }
            return hours + " часов";
        }

        public static string GetMinutesString(int minutes)
        {
            return minutes + " мин";
        }

        public static string GetSecondsString(int seconds)
        {
            return seconds + " сек";
        }

        public static string GetOneUnitString(TimeSpan timeSpan)
        {
            int days = (int)timeSpan.TotalDays;
            
            if (days >= 1)
            {
                return GetDaysString(days);
            }
            if (timeSpan.Hours > 0)
            {
                return GetHoursString(timeSpan.Hours);
            }
            if (timeSpan.Minutes > 0)
            {
                return GetMinutesString(timeSpan.Minutes);
            }
            return GetSecondsString(timeSpan.Seconds);
        }

        public static string Get2UnitsString(TimeSpan timeSpan)
        {
            int days = (int) timeSpan.TotalDays;

            // 5 дней
            if (days >= 5)
            {
                return GetDaysString(days);
            }

            // 2 для 5 часов
            if (days >= 1)
            {
                if (timeSpan.Hours > 0)
                {
                    return GetDaysString(days) + " " + GetHoursString(timeSpan.Hours);
                }
                return GetDaysString(days);
            }

            // 5 часов
            if (timeSpan.Hours >= 5)
            {
                return GetHoursString(timeSpan.Hours);
            }

            // 2 часа 30 мин
            if (timeSpan.Hours >= 1)
            {
                if (timeSpan.Minutes > 0)
                {
                    return GetHoursString(timeSpan.Hours) + " " + GetMinutesString(timeSpan.Minutes);
                }
                return GetHoursString(timeSpan.Hours);
            }

            // 5 мин
            if (timeSpan.Minutes >= 5)
            {
                return GetMinutesString(timeSpan.Minutes);
            }

            // 2 мин 5 сек
            if (timeSpan.Minutes >= 1)
            {
                if (timeSpan.Seconds > 0)
                {
                    return GetMinutesString(timeSpan.Minutes) + " " + GetSecondsString(timeSpan.Seconds);
                }
                return GetMinutesString(timeSpan.Minutes);
            }

            // 30 сек
            return GetSecondsString(timeSpan.Seconds);
        }

        /// <summary>
        /// Парсит строку вида: 1h
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeSpan ParseHtml(string value)
        {
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            var list = value.Split(' ');

            foreach (var item in list)
            {
                var unit = item.ToLowerInvariant()[item.Length - 1];
                if (unit == 'd' || unit == 'д')
                {
                    if (!int.TryParse(item.Substring(0, item.Length - 1), out days))
                    {
                        throw new FormatException();
                    }
                }
                else if (unit == 'h' || unit == 'ч')
                {
                    if (!int.TryParse(item.Substring(0, item.Length - 1), out hours))
                    {
                        throw new FormatException();
                    }
                }
                else if (unit == 'm' || unit == 'м')
                {
                    if (!int.TryParse(item.Substring(0, item.Length - 1), out minutes))
                    {
                        throw new FormatException();
                    }
                }
                else if (unit == 's' || unit == 'с' || unit == 'c')
                {
                    if (!int.TryParse(item.Substring(0, item.Length - 1), out seconds))
                    {
                        throw new FormatException();
                    }
                }
                else
                {
                    throw new FormatException();
                }
            }
            return new TimeSpan(days, hours, minutes, seconds);
        }
    }
}
