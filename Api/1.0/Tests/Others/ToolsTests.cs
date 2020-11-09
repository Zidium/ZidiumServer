using System;
using Zidium.Api;
using Xunit;

namespace ApiTests_1._0.Others
{
    public class ToolsTests : BaseTest
    {
        [Fact]
        public void ManualExceptionJoinKeyTest()
        {
            //проверим, что созданное вручную исключение не приведет к ошибке
            var exception = new Exception("hello");
            long key = Tools.GetJoinKey(exception);
            Assert.NotEqual(0, key);
        }

        [Fact]
        public void GetApplicationNameNoClientTest()
        {
            var appName = Tools.GetApplicationName(null);
            Assert.False(string.IsNullOrEmpty(appName));
        }

        [Fact]
        public void GetApplicationNameWithClientTest()
        {
            var client = new Client();
            var appName = Tools.GetApplicationName(client);
            Assert.False(string.IsNullOrEmpty(appName));
        }

        [Fact]
        public void OutOfMemoryExceptionInfoTest()
        {
            var exception = new OutOfMemoryException();

            Tools.HandleOutOfMemoryException(exception);

            var memoryInfo = exception.Data["MemoryInfo"].ToString();
            Assert.False(string.IsNullOrEmpty(memoryInfo));

            var linesCount = memoryInfo.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Length;
            Assert.Equal(3, linesCount);
        }

        [Fact]
        public void GetProgramNameTest()
        {
            var programName = Tools.GetProgramName();
            Assert.NotNull(programName);
        }
    }
}
