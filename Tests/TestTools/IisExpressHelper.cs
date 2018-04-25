using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Zidium.TestTools
{
    public class IisExpress
    {
        public static void StartApiWebService()
        {
            string dir = typeof(IisExpress).Assembly.CodeBase;
            dir = dir.Substring(8);
            dir = Path.GetDirectoryName(dir);
            dir = dir.Substring(0, dir.Length - @"\Tests\bin\Debug".Length);
            dir = Path.Combine(dir, "ApiHttpService");
            Start(dir, 61000);
        }

        public static void Start(string appPath, int port)
        {
            var thread = new Thread(() => StartIisExpress(appPath, port)) { IsBackground = true };
            thread.Start();
        }

        private static void StartIisExpress(string appPath, int port)
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = string.Format("/path:\"{0}\" /port:{1}", appPath, port)
            };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                                ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            var _iisProcess = new Process { StartInfo = startInfo };
            try
            {
                _iisProcess.Start();
                _iisProcess.WaitForExit();
            }
            catch
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }
    }
}
