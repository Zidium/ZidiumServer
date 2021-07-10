using System.Text;
using System.Web;
using Zidium.Api.Dto;
using Zidium.Api.Others;
using Zidium.Storage;

namespace Zidium.Core.Common
{
    public class HtmlRender
    {
        protected StringBuilder StringBuilder { get; set; }

        protected const string Tab = "    "; // 4 пробела

        public HtmlRender()
        {
            StringBuilder = new StringBuilder();
        }

        public string GetHtml()
        {
            return StringBuilder.ToString();
        }

        public static string HtmlEncode(string content)
        {
            return HttpUtility.HtmlEncode(content);
        }

        public void Write(string content)
        {
            string encodedText = HtmlEncode(content);
            StringBuilder.Append(encodedText);
        }

        public void WriteRaw(string content)
        {
            StringBuilder.Append(content);
        }

        public void Span(string content, string style = null)
        {
            var encodedText = HtmlEncode(content);
            StringBuilder.Append("<span" + (style != null ? " style='" + style + "' " : "") + ">" + encodedText + "</span>");
        }

        public void BeginRow()
        {
            StringBuilder.AppendLine(Tab + "<tr style='vertical-align: top;'>");
        }

        public void EndRow()
        {
            StringBuilder.AppendLine(Tab + "</tr>");
        }

        public void WriteCell(string content, bool preFormat = false)
        {
            string encoded = HtmlEncode(content);
            WriteCellRaw(encoded, preFormat);
        }

        public void WriteCellRaw(string content, bool preFormat = false)
        {
            StringBuilder.AppendLine(Tab + Tab + "<td style='border-top: 1px solid lightgray; padding: 5px;'>" + (preFormat ? "<span style='margin-top: 0px; margin-bottom: 0px;white-space: pre-wrap;word-break: break-all;'>" : "") + content + (preFormat ? "</span>" : "") + "</td>");
        }

        public void WriteExtentionPropertyRow(string name, string value, DataType type, bool preFormat = false)
        {
            if (type == DataType.Binary)
            {
                value = "бинарные данные"; //todo
            }
            value = StringHelper.TrimLenght(value, 4000);// обрежем, чтобы не было слишком больших ячеек
            if (!preFormat)
                WriteCells(name, value);
            else
            {
                BeginRow();
                WriteCell(name);
                WriteCell(value, true);
                EndRow();

            }
        }

        public void WriteCells(params string[] cellContents)
        {
            BeginRow();
            foreach (string content in cellContents)
            {
                WriteCell(content);
            }
            EndRow();
        }

        public void WriteCellsPreformat(params string[] cellContents)
        {
            BeginRow();
            foreach (string content in cellContents)
            {
                WriteCell(content, true);
            }
            EndRow();
        }

        public void WriteRawCells(params string[] cellContents)
        {
            BeginRow();
            foreach (string content in cellContents)
            {
                WriteCellRaw(content);
            }
            EndRow();
        }

        public void BeginTable()
        {
            StringBuilder.AppendLine("<table style='border-collapse: collapse;'>");
        }

        public void EndTable()
        {
            StringBuilder.AppendLine("</table>");
        }

        public void WriteLink(string href, string title)
        {
            string linkHtml = GetLinkHtml(href, title);
            StringBuilder.Append(linkHtml);
        }

        public static string GetLinkHtml(string href, string title)
        {
            string encoded = HtmlEncode(title);
            return string.Format("<a href='{0}'>{1}</a>", href, encoded);
        }

        public void NewLine()
        {
            StringBuilder.AppendLine("<br/>");
        }
    }
}
