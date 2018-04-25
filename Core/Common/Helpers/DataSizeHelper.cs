using System;
using System.Globalization;
using System.Linq;

namespace Zidium.Core.Common.Helpers
{
    public static class DataSizeHelper
    {
        public const long GByte = 1024 * 1024 * 1024; // long чтобы не было переполнения при умножении int32 * int32

        public const long MByte = 1024 * 1024; // long чтобы не было переполнения при умножении int32 * int32

        public const long KByte = 1024; // long чтобы не было переполнения при умножении int32 * int32

        /// <summary>
        /// Размер данных 1 записи в БД = 250 байт
        /// Статистика от 19.06.2016
        /// </summary>
        public static readonly int OneMetricSize = 250;

        /// <summary>
        /// Размер данных 1 экземпляра события (вместе с доп. свойствами) = 4 Кбайта
        /// Значение получено по статистике в БД
        /// Статистика от 19.06.2016
        /// </summary>
        public static readonly int OneEventAverageSize = 4096;

        /// <summary>
        /// Размер данных 1 экземпляра события "Результат проверки" (вместе с доп. свойствами)
        /// 1 Кбайт - взято наобум ... 
        /// Земляникин: в большинстве случаев проверка успешна, в ней только "ОК" слово
        /// </summary>
        public static readonly int OneUnitTestResultAverageSize = 4096;

        /// <summary>
        /// Размер 1 символа в байтах
        /// Считаем, что в БД используется UTF-16
        /// </summary>
        public static readonly int OneCharSize = 2;

        public static string GetSizeText(long value)
        {
            if (value == long.MaxValue)
                return "нет лимита";

            // вычислим ед. измерения
            var units = new[] {"байт", "КБ", "МБ", "ГБ"};
            string currentUnit = null;
            var lastUnit = units.Last();
            double round = value;
            foreach (var unit in units)
            {
                currentUnit = unit;
                if (round < 1024)
                {
                    break;
                }
                if (unit != lastUnit)
                {
                    round = round / 1024;
                }
            }

            // округлим
            if (round < 10)
            {
                round = Math.Round(round, 2);
            }
            else if (round < 100)
            {
                round = Math.Round(round, 1);
            }
            else
            {
                round = (long)round;
            }
            return NumbersHelper.Amount(round) + " " + currentUnit;
        }

        // Размеры служебных данных в таблицах
        // Статистика от 16.11.2017

        public static readonly int DbLogRecordOverhead = 500;

        public static readonly int DbLogParameterRecordOverhead = 2500;

        public static readonly int DbMetricRecordOverhead = 500;

        public static readonly int DbEventRecordOverhead = 1000;

        public static readonly int DbEventParameterRecordOverhead = 1100;

        public static readonly int DbEventStatusRecordOverhead = 500;


    }
}
