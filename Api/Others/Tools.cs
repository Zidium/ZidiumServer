using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyModel;

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

        public static string GetApplicationDir()
        {
            if (_applicationDir == null)
            {
                _applicationDir = Directory.GetCurrentDirectory();
            }
            return _applicationDir;
        }

        private static string _applicationDir;

        public static string GetLocalAppDataDir()
        {
            if (_localAppDataDir == null)
            {
                _localAppDataDir = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalAppData" : "Home");
            }
            return _localAppDataDir;
        }

        private static string _localAppDataDir;

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

        public static Assembly[] GetAllAssemblies()
        {
            var assemblyNames = DependencyContext.Default.CompileLibraries.Select(t => t.Name)
                .Where(t => !t.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) && !t.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return assemblyNames.Select(t =>
                {
                    try
                    {
                        return Assembly.Load(new AssemblyName(t));
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(t => t != null)
                .Where(t =>
                {
                    var attributeData = t.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();
                    if (attributeData == null)
                        return true;
                    return !attributeData.Product.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);
                })
                .ToArray();
        }

    }
}
