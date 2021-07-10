using System.Net.Sockets;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.UnitTests.TcpPortChecks
{
    public class TcpPortCheckProcessor
    {
        public TcpPortCheckOutputData Process(TcpPortCheckInputData inputData)
        {
            // проверка порта
            if (inputData.Port < 0 || inputData.Port > 65535)
            {
                return new TcpPortCheckOutputData()
                {
                    Code = TcpPortCheckCode.InvalidPort,
                    Message = "Неверное значение TCP порта"
                };
            }

            // проверка IP
            bool hasIP = NetworkHelper.IsDomainHasIp(inputData.Host);
            if (hasIP == false)
            {
                return new TcpPortCheckOutputData()
                {
                    Code = TcpPortCheckCode.NoIp,
                    Message = "Не удалось получить IP адресс"
                };
            }
            
            // проверка соединения
            try
            {
                using (var client = new TcpClient())
                {
                    //int timeoutMs = (int)inputData.Timeout.TotalMilliseconds;
                    //client.SendTimeout = timeoutMs;
                    //client.ReceiveTimeout = timeoutMs;
                    client.Connect(inputData.Host, inputData.Port);
                    if (client.Connected)
                    {
                        return new TcpPortCheckOutputData()
                        {
                            Code = TcpPortCheckCode.Opened,
                            Message = "Порт открыт"
                        };
                    }

                    return new TcpPortCheckOutputData()
                    {
                        Code = TcpPortCheckCode.Closed,
                        Message = "Порт закрыт"
                    };
                }
            }
            catch (SocketException exception)
            {
                if (exception.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return new TcpPortCheckOutputData()
                    {
                        Code = TcpPortCheckCode.Closed,
                        Message = "Порт закрыт"
                    };
                }
                if (exception.SocketErrorCode == SocketError.TimedOut)
                {
                    return new TcpPortCheckOutputData()
                    {
                        Code = TcpPortCheckCode.Closed,
                        Message = "Порт закрыт"
                    };
                }
                return new TcpPortCheckOutputData()
                {
                    Code = TcpPortCheckCode.UnknownError,
                    Message = exception.Message
                };
            }
        }
    }
}
