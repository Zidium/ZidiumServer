using System;
using System.Globalization;

namespace Zidium.Core.Common
{
    public static class ParseHelper
    {
        /// <summary>
        /// Парсит урл сайта, если не получается возвращает NULL
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri TryParseWebSiteUri(string uri)
        {
            try
            {
                if (uri == null)
                {
                    return null;
                }
                if (uri.Contains(".") == false)
                {
                    return null;
                }
                if (uri.Contains("://") == false)
                {
                    uri = "http://" + uri;
                }
                return new Uri(uri);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        public static DateTime ParseDateTime(string value)
        {
            DateTime? result = TryParseDateTime(value);
            if (result.HasValue == false)
            {
                throw new FormatException("Неверный формат даты: '" + value + "'");
            }
            return result.Value;
        }

        public static DateTime? TryParseDateTime(string value)
        {
            if (value == null)
            {
                return null;
            }
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            culture.DateTimeFormat.DateSeparator = "/";

            // проверим формат веб-форм = dd.MM.yyyy_HH.mm.ss
            if (value.Contains("_"))
            {
                var strs = value.Split('_');
                value = strs[0] + " " + strs[1].Replace(".", ":");
            }
            else if (value.Length > 10 && value[10] == '.') // проверим старый формат веб-форм = dd.MM.yyyy.HH.mm.ss
            {
                var strs = value.Substring(11);
                value = value.Substring(0, 10) + " " + strs.Replace(".", ":");
            }
            value = value.Replace('.', '/');
            
            DateTime result;
            if (DateTime.TryParse(value, culture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException("Значение не указано");
            }
            // .Replace(" ", "") - чтобы можно было парсить "1 000 0000"
            return double.Parse(value.Replace(" ", "").Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        public static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException("Значение не указано");
            }
            // .Replace(" ", "") - чтобы можно было парсить "1 000 0000"
            return decimal.Parse(value.Replace(" ", "").Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}
