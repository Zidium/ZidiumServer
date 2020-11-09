using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.TcpPortChecks;

namespace Zidium.Core.Tests.AgentTests
{
    public class TcpPortCheckTests : BaseTest
    {
        [Fact]
        public void TestOpened()
        {
            TcpPortCheckProcessor processor = new TcpPortCheckProcessor();
            var inputData = new TcpPortCheckInputData()
            {
                Port = 80,
                Host = "recursion.ru"
            };
            var outputData = processor.Process(inputData);
            Assert.Equal(TcpPortCheckCode.Opened, outputData.Code);
        }

        [Fact]
        public void TestClosed()
        {
            TcpPortCheckProcessor processor = new TcpPortCheckProcessor();
            var inputData = new TcpPortCheckInputData()
            {
                Port = 82,
                Host = "recursion.ru"
            };
            var outputData = processor.Process(inputData);
            Assert.Equal(TcpPortCheckCode.Closed, outputData.Code);
        }

        [Fact]
        public void TestInvalidPort()
        {
            TcpPortCheckProcessor processor = new TcpPortCheckProcessor();
            var inputData = new TcpPortCheckInputData()
            {
                Port = 660000,
                Host = "recursion.ru"
            };
            var outputData = processor.Process(inputData);
            Assert.Equal(TcpPortCheckCode.InvalidPort, outputData.Code);
        }

        [Fact]
        public void TestNoIp()
        {
            TcpPortCheckProcessor processor = new TcpPortCheckProcessor();
            var inputData = new TcpPortCheckInputData()
            {
                Port = 6600,
                Host = "recursion22.ru"
            };
            var outputData = processor.Process(inputData);
            Assert.Equal(TcpPortCheckCode.NoIp, outputData.Code);
        }
    }
}
