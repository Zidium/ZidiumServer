using System;
using System.Linq;

namespace Zidium.Core.Common.Helpers
{
    public static class DataSizeHelper
    {
        public const long GByte = 1024 * 1024 * 1024; // long чтобы не было переполнения при умножении int32 * int32

        public const long MByte = 1024 * 1024; // long чтобы не было переполнения при умножении int32 * int32

        public const long KByte = 1024; // long чтобы не было переполнения при умножении int32 * int32

        public static string GetSizeText(long value)
        {
            if (value == long.MaxValue)
                return "нет лимита";

            // вычислим ед. измерения
            var units = new[] { "байт", "КБ", "МБ", "ГБ" };
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
                    round /= 1024;
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

    }
}
