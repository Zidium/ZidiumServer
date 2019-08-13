using System;
using System.Globalization;
using System.IO;

namespace Zidium.Core.Common.Helpers
{
    public class CsvWriter
    {
        public CsvWriter(TextWriter writer)
        {
            _writer = writer;
        }

        private readonly TextWriter _writer;

        private bool _isNewLine = true;

        private void WriteCell()
        {
            if (!_isNewLine)
                _writer.Write(";");
            _isNewLine = false;
        }

        public void WriteCell(string value)
        {
            WriteCell();
            _writer.Write(value != null ? Quote(value.Replace("\"", "\"\"")) : "");
        }

        public void WriteCell(double? value)
        {
            WriteCell();
            _writer.Write(value != null ? Quote(value.Value.ToString("#,0.##", DoubleCultureInfo)) : "");
        }

        public void WriteCell(Int64? value)
        {
            WriteCell();
            _writer.Write(value != null ? Quote(value.Value.ToString("#,0#", DoubleCultureInfo)) : "");
        }

        public void WriteCell(bool? value)
        {
            WriteCell();
            if (value == null)
            {
                _writer.Write("");
            }
            else if (value == true)
            {
                _writer.Write(Quote("Да"));
            }
            else
            {
                _writer.Write(Quote("Нет"));
            }
        }

        public void WriteCell(DateTime? value)
        {
            WriteCell();
            _writer.Write(Quote(DateTimeHelper.GetRussianDateTime(value)));
        }

        public void WriteNewLine()
        {
            _writer.WriteLine();
            _isNewLine = true;
        }

        private string Quote(string value)
        {
            return "\"" + value + "\"";
        }

        internal static CultureInfo DoubleCultureInfo;

        static CsvWriter()
        {
            DoubleCultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DoubleCultureInfo.NumberFormat.NumberGroupSeparator = " ";
            DoubleCultureInfo.NumberFormat.NumberGroupSizes = new[] { 3 };
            DoubleCultureInfo.NumberFormat.NumberDecimalSeparator = ".";
        }
    }
}
