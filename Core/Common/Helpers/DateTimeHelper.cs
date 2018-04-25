using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Zidium.Api;

namespace Zidium.Core.Common.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Округляем дату до целого количества минут, которые делеятся на 5.
        /// Округление идет в большую сторону (вправо).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime Round5MinutesRight(DateTime value)
        {
            int minutes = (value.Minute / 5) * 5;
            var result = new DateTime(value.Year, value.Month, value.Day, value.Hour, minutes, 0);
            if (result < value)
            {
                result = result.AddMinutes(5);
            }
            return result;
        }

        /// <summary>
        /// Округляем дату до ровного количества часов (минуты  и секунды отбрасываются)
        /// Округление идет в большую сторону (вправо).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime Round1HourRight(DateTime value)
        {
            var result = new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
            if (result < value)
            {
                result = result.AddHours(1);
            }
            return result;
        }

        /// <summary>
        /// Возвращает дату и время без Мс
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime TrimMs(DateTime value)
        {
            return new DateTime(
                value.Year, 
                value.Month, 
                value.Day, 
                value.Hour, 
                value.Minute, 
                value.Second);
        }

        public static DateTime Min(params DateTime[] values)
        {
            return values.OrderBy(x => x).First();
        }

        public static DateTime Max(params DateTime[] values)
        {
            return values.OrderBy(x => x).Last();
        }

        /// <summary>
        /// Ждем наступления указанного времени
        /// </summary>
        /// <param name="time"></param>
        public static void WaitForTime(DateTime time)
        {
            while (DateTime.Now < time)
            {
                Thread.Sleep(50);
            }
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static string GetRussianDate(DateTime? date)
        {
            if (date == null)
                return string.Empty;
            return date.Value.ToString(DateTimeFormat.RUS_DDMMYYYY);
        }

        public static string GetRussianDateTime(DateTime? date)
        {
            if (date == null)
                return string.Empty;
            return date.Value.ToString(DateTimeFormat.RUS_DDMMYYYY_HHMMSS);
        }

        public static string ToUrlFormat(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            return ToUrlFormat(date.Value);
        }

        public static string ToUrlFormat(DateTime date)
        {
            return date.ToString(DateTimeFormat.UrlFormat);
        }

        private static DateTime ParseUrlDate(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            var parts = text.Split('-');
            if (parts.Length != 3)
            {
                throw new FormatException("Неверный URL-формат даты");
            }
            int year = int.Parse(parts[0], CultureInfo.InvariantCulture);
            if (year < 1 || year > 3000)
            {
                throw new FormatException("Неверный URL-формат даты");
            }
            int month = int.Parse(parts[1], CultureInfo.InvariantCulture);
            if (month < 1 || month > 12)
            {
                throw new FormatException("Неверный URL-формат даты");
            }
            int day = int.Parse(parts[2], CultureInfo.InvariantCulture);
            if (day < 1 || day > 31)
            {
                throw new FormatException("Неверный URL-формат даты");
            }
            return new DateTime(year, month, day);
        }

        public static DateTime FromUrlFormat(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Внимание!
            // Дата может придти в системном формате (G), если она вставлялась в url с помощью
            // Url.Action(..., routeValues: new { date = MyDate } )
            // Сначала попытаемся преобразовать дату из системного формата
            DateTime result;
            if (DateTime.TryParseExact(text, "G", CultureHelper.Russian, DateTimeStyles.None, out result))
            {
                return result;
            }

            // В редких непонятных случаях mvc форматирует дату с использованием InvariantCulture, а не берёт из текущего потока 
            if (DateTime.TryParseExact(text, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            
            // если только дата (без времени)
            if (text.Length == 10)
            {
                return ParseUrlDate(text);
            }
            var parts = text.Split('_');
            if (parts.Length != 2)
            {
                throw new FormatException("Неверный URL-формат даты");
            }

            // строка содержит дату и время

            // получим дату
            var date = ParseUrlDate(parts[0]);

            // получим время
            parts = parts[1].Split('.');
            if (parts.Length > 3)
            {
                throw new FormatException("Неверный URL-формат даты");
            }
            int hours = int.Parse(parts[0], CultureInfo.InvariantCulture);
            int minutes = 0;
            int seconds = 0;
            if (parts.Length > 1)
            {
                minutes = int.Parse(parts[1], CultureInfo.InvariantCulture);
                if (minutes < 0 || minutes > 59)
                {
                    throw new FormatException("Неверный URL-формат даты");
                }
            }
            if (parts.Length > 2)
            {
                seconds = int.Parse(parts[2], CultureInfo.InvariantCulture);
                if (seconds < 0 || seconds > 59)
                {
                    throw new FormatException("Неверный URL-формат даты");
                }
            }
            return new DateTime(date.Year, date.Month, date.Day, hours, minutes, seconds);
        }
    }
}
