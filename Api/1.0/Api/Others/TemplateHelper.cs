using System;
using System.Collections.Generic;

namespace Zidium.Api.Others
{
    /// <summary>
    /// Заполняет строковый шаблон значениями (используется в конфигах)
    /// Например, 
    /// template = #appDir\Logs\#date\myProgram_#date_#hour.txt
    /// result = C:\MyProgramDir\Logs\2015.05.05\myProgram_2015.05.05_09.txt
    /// </summary>
    public static class TemplateHelper
    {
        private static string GetResultInternal(string template, Dictionary<string, Func<string>> valuesFuncs)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }
            var result = template;
            var comparison = StringComparison.OrdinalIgnoreCase;
            foreach (var valuesFunc in valuesFuncs)
            {
                string key = valuesFunc.Key;
                if (StringHelper.Contains(template, key, comparison))
                {
                    string value = valuesFunc.Value();
                    if (value == null)
                    {
                        value = string.Empty;
                    }

                    result = StringHelper.ReplaceComparison(
                        result,
                        key,
                        value,
                        comparison);
                }
            }
            result = result.Trim();
            return result;
        }

        public static string GetLogPath(string template, IClient client)
        {
            var values = new Dictionary<string, Func<string>>()
            {
                {"#appDir", Tools.GetApplicationDir},
                {"#appName", () => Tools.GetApplicationName(client)},
                {"#date", () => DateTime.Now.ToString("yyyy.MM.dd")},
                {"#year", () => DateTime.Now.ToString("yyyy")},
                {"#month", () => DateTime.Now.ToString("MM")},
                {"#day", () => DateTime.Now.ToString("dd")},
                {"#hour", () => DateTime.Now.ToString("HH")}
            };
            return GetResultInternal(template, values);
        }

        private static string GetPropertiesText(ExtentionPropertyCollection properties)
        {
            if (properties == null || properties.Count == 0)
            {
                return "";
            }
            string result = "";
            foreach (var property in properties)
            {
                var bytes = property.Value.Value as byte[];
                if (bytes != null)
                {
                    result += property.Name + "=" + bytes.Length + "bytes; ";
                }
                else
                {
                    result += property.Name + "=" + property.Value + "; ";
                }
            }
            result = result.TrimEnd(';', ' ');
            result = "[" + result + "]";
            return result;
        }

        public static string GetLogMessage(
            string template, 
            string componentName,
            DateTime date,
            LogLevel? level,
            string tag,
            string message,
            ExtentionPropertyCollection properties)
        {
            var values = new Dictionary<string, Func<string>>()
            {
                {"#tag", () => tag},
                {"#componentName", () => componentName},
                {"#datetime", () => date.ToString("dd.MM.yyyy HH:mm:ss")},
                {"#level", () => (level == null) ? "       " : level.ToString().PadRight(7, ' ')},
                {"#message", () => message},
                {"#properties", () => GetPropertiesText(properties)}
            };
            return GetResultInternal(template, values);
        }

        public static string GetResult(string template, Dictionary<string, Func<string>> valuesFuncs)
        {
            return GetResultInternal(template, valuesFuncs);
        }
    }
}
