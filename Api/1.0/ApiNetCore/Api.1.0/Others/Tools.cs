using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Zidium.Api
{
    public static class Tools
    {
        public static string GetExceptionFullMessage(Exception exception)
        {
            var render = new ExceptionRender();
            return render.GetMessage(exception);
        }

        public static long GetJoinKey(Exception exception)
        {
            return HashHelper.GetInt64(
                exception.GetType().FullName,
                exception.StackTrace);
        }

        /// <summary>
        /// True - если текущее приложение является веб-приложением
        /// </summary>
        /// <returns></returns>
        public static bool IsWebApplication()
        {
            var appDir = GetApplicationDir();
            return IsWebApplicationDir(appDir);
        }

        private static bool IsWebApplicationDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return false;
            }
            if (Directory.Exists(dir) == false)
            {
                return false;
            }
            var files = new DirectoryInfo(dir).GetFiles();
            if (files.Length == 0)
            {
                return false;
            }
            var webFiles = new[] { "web.config", "global.asax" };
            var hasWebFiles = files.Any(x => webFiles.Contains(x.Name.ToLower()));
            return hasWebFiles;
        }

        public static string GetApplicationDir()
        {
            var dir = Directory.GetCurrentDirectory();
            if (dir != null && dir.EndsWith(@"\bin", StringComparison.OrdinalIgnoreCase))
            {
                var parentDir = dir.Substring(0, dir.Length - 4);
                var isWebApplication = IsWebApplicationDir(parentDir);
                if (isWebApplication)
                {
                    return parentDir;
                }
            }
            return dir;
        }

        public static string GetApplicationName(IClient client)
        {
            string result = null;

            if (client != null)
                result = client.AccessToken.Program;

            if (string.IsNullOrEmpty(result))
            {
                result = GetProgramName();
            }
            return result;
        }

        public static void HandleOutOfMemoryException(Exception exception)
        {
            if (!(exception is OutOfMemoryException))
                return;

            var memoryInfo = GetMemoryInfo();
            if (memoryInfo != null)
                exception.Data.Add("MemoryInfo", memoryInfo);
        }

        internal static string GetMemoryInfo()
        {
            var sb = new StringBuilder();

            try
            {
                using (var proc = Process.GetCurrentProcess())
                {
                    sb.AppendLine("Used by this application: " + proc.PagedMemorySize64);
                    sb.AppendLine("Peak used by this application: " + proc.PeakPagedMemorySize64);
                }

                // TODO Get available memory size
                /*
                var counter = new PerformanceCounter("Memory", "Available Bytes");
                var value = (long) counter.NextValue();
                sb.AppendLine("Available: " + value);
                */
                sb.AppendLine("Available: unknown");
            }
            catch (Exception e)
            {
                sb.AppendLine("Error getting memory info: " + e.Message);
            }

            return sb.ToString();
        }

        public static string GetProgramName()
        {
            try
            {
                var startAssembly = Assembly.GetEntryAssembly();
                return Path.GetFileName(startAssembly.Location);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static Dictionary<string, object> GetPropertyCollection(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var properties = obj.GetType().GetTypeInfo().GetProperties();
            var dictionary = new Dictionary<string, object>();
            foreach (var property in properties)
            {
                if (property.CanRead == false)
                {
                    continue;
                }
                string name = property.Name;
                object value = property.GetValue(obj, null);
                dictionary.Add(name, value);
            }
            return dictionary;
        }

        public static string ApiVersion
        {
            get
            {
                if (_apiVersion == null)
                {
                    _apiVersion = typeof(Tools).GetTypeInfo().Assembly.GetName().Version.ToString();
                }
                return _apiVersion;
            }
        }

        private static string _apiVersion;
    }
}
