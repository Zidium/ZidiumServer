using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if PocketPC
using System.Reflection;
#endif

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
            var webFiles = new[] {"web.config", "global.asax"};
            var hasWebFiles = files.Any(x => webFiles.Contains(x.Name.ToLower()));
            return hasWebFiles;
        }

        public static string GetApplicationDir()
        {
            #if !PocketPC
            var dir = typeof(Tools).Assembly.CodeBase;
            dir = dir.Substring(8);
            #else
            var dir = typeof(Tools).Assembly.GetName().CodeBase;
            #endif

            dir = Path.GetDirectoryName(dir);
            if (dir != null && dir.EndsWith(@"\bin", StringComparison.InvariantCultureIgnoreCase))
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
                #if !PocketPC
                var exeName = Environment.GetCommandLineArgs()[0];
                #else
                var exeName = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                #endif

                result = Path.GetFileName(exeName);
            }
            return result;
        }

        /// <summary>
        /// Добавление в Exception.Data информации о текущем состоянии памяти
        /// </summary>
        public static void HandleOutOfMemoryException(Exception exception)
        {
            if (!(exception is OutOfMemoryException))
                return;
            
            var memoryInfo = GetMemoryInfo();

            if (memoryInfo != null)
                exception.Data.Add("MemoryInfo", memoryInfo);
        }

        #if !PocketPC
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

                var counter = new PerformanceCounter("Memory", "Available Bytes");
                var value = (long) counter.NextValue();
                sb.AppendLine("Available: " + value);
            }
            catch (Exception e)
            {
                sb.AppendLine("Error getting memory info: " + e.Message);
            }

            return sb.ToString();
        }
#else
        internal static string GetMemoryInfo()
        {
            return null;
        }
#endif

        private static bool IsSystemType(Type type)
        {
            if (type == null)
                return true;

            var systems = new[]
            {
                "System",
                "Microsoft",
                "XUnit",
                "NUnit"
            };

            foreach (var system in systems)
            {
                // проверяет полное совпадение (нужно для mscorlib)
                if (string.Equals(system, type.Namespace, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // проверяет, что начинается с "System."
                if (type.Namespace != null && type.Namespace.StartsWith(system + ".", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetProgramName()
        {
            try
            {
#if !PocketPC
                var callStack = new StackTrace();
                var frames = callStack.GetFrames().Reverse();
                var myFrame = frames.FirstOrDefault(x => !IsSystemType(x.GetMethod().ReflectedType));
                if (myFrame == null)
                    myFrame = frames.First();
                var startAssembly = myFrame.GetMethod().ReflectedType.Assembly;
                return Path.GetFileName(startAssembly.Location);
#else
                return Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
#endif
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
            var properties = obj.GetType().GetProperties();
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
                    _apiVersion = typeof(Tools).Assembly.GetName().Version.ToString();
                }
                return _apiVersion;
            }
        }

        private static string _apiVersion;

        public static Assembly[] GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(t =>
            {
                var attributeData = (AssemblyProductAttribute)t.GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault();
                if (attributeData == null)
                    return true;
                return !attributeData.Product.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);
            }).ToArray();
        }

    }
}
