using System;
using System.Linq;

namespace Zidium.UserAccount.Models.Controls
{
    public class SimpleBarCharModel
    {
        public class DataItem
        {
            /// <summary>
            /// Номер столбца (начиается с нуля)
            /// </summary>
            public int Index { get; set; }

            public DateTime FromTime { get; set; }

            public DateTime ToTime { get; set; }

            public string ValueTitle { get; set; }

            public string GetTitle()
            {
                if (string.IsNullOrEmpty(ValueTitle)==false)
                {
                    return ValueTitle;
                }
                return Value.ToString();
            }

            public int Value { get; set; }

            public string Url { get; set; }

            public string UrlTitle { get; set; }
        }

        public DataItem[] Items { get; set; }

        public string ColumnWidth { get; protected set; }

        protected double HeightK { get; set; }

        public SimpleBarCharModel(DataItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            Items = items;
            Init();
        }

        private void Init()
        {
            double columnWidth = 99 / (double)Items.Length;
            ColumnWidth = Math.Round(columnWidth, 4) + "%";
            int maxValue = Items.Max(x => x.Value);
            if (maxValue < 20)
            {
                HeightK = 3;
            }
            else
            {
                HeightK = (double) maxValue / 100;
            }
        }

        public string GetColumnHeight(int value)
        {
            return Math.Round(HeightK * value, 3) + "%";
        }
    }
}