using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Zidium.Core.Common.Helpers
{
    public static class NetworkHelper
    {
        private static DateTime _nextInternetWorkCheckDate;
        private static bool _isInternetWorkValue;
        private static readonly object IsInternetWorkSynchObject = new object();

        private static bool HasSuccessPing(string host)
        {
            if (IsDomainHasIp(host) == false)
            {
                return false;
            }
            try
            {
                var ping = new Ping();
                const int timeoutMs = 2000;
                var reply = ping.Send(host, timeoutMs);
                return reply!= null && reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public static string GetHttpResponse(string url, TimeSpan timeout)
        {
            int timeoutMs = (int) timeout.TotalMilliseconds;
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            // запрос
            var uri = new Uri(url);
            var request = WebRequest.Create(uri) as HttpWebRequest;

            if (request == null)
                return null;

            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 40;
            request.Timeout = timeoutMs;
            request.ReadWriteTimeout = timeoutMs;

            // ответ
            var response = (HttpWebResponse) request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream == null)
                return null;

            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
            var html = streamReader.ReadToEnd();
            return html;
        }

        private static bool IsPageLoad(string url)
        {
            var timeOut = TimeSpan.FromSeconds(3);
            try
            {
                string html = GetHttpResponse(url, timeOut);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет работает ли Интернет с помощью пинга и http-запроса
        /// </summary>
        /// <returns></returns>
        private static bool IsInternetWorkInternal()
        {
            var pingHosts = new[]
            {
                "google.ru",
                "yandex.ru"
            };
            var httpRequests = new[]
            {
                "http://yandex.ru",
                "http://google.ru"
            };
            foreach (var host in pingHosts)
            {
                if (HasSuccessPing(host))
                {
                    foreach (var url in httpRequests)
                    {
                        if (IsPageLoad(url))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет, что интернет работает
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetWork()
        {
            // делаем реальную проверку интернета не чаще 1 раза в 5 секунд
            // и кэш используем только, если интернет НЕ работает
            // значение true вычисляется каждый раз явно!
            var now = DateTime.Now;
            if (_isInternetWorkValue == false && now < _nextInternetWorkCheckDate)
            {
                return _isInternetWorkValue;
            }
            lock (IsInternetWorkSynchObject)
            {
                if (_isInternetWorkValue == false && now < _nextInternetWorkCheckDate)
                {
                    return _isInternetWorkValue;
                }
                var result = IsInternetWorkInternal();
                _nextInternetWorkCheckDate = DateTime.Now.AddSeconds(5);
                _isInternetWorkValue = result;
                return result;
            }
        }

        public static bool IsIpAddress(string ip)
        {
            IPAddress result;
            if (IPAddress.TryParse(ip, out result))
            {
                return true;
            }
            return false;
        }

        public static string GetIpFromHost(string host)
        {
            try
            {
                if (IsIpAddress(host))
                {
                    return host;
                }
                var hostEntry = Dns.GetHostEntry(host);
                var ip = hostEntry.AddressList.FirstOrDefault();
                if (ip == null)
                {
                    return null;
                }
                return ip.ToString();
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public static bool IsDomainHasIp(string host)
        {
            try
            {
                if (IsIpAddress(host))
                {
                    return true;
                }
                var hostEntry = Dns.GetHostEntry(host);
                return hostEntry.AddressList.Length > 0;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public static string GetLocalIp()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                if (endPoint == null)
                    return "127.0.0.1";
                return endPoint.Address.ToString();
            }
        }
    }
}
