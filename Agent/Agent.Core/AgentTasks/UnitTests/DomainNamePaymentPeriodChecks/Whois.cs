using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Zidium.Agent.AgentTasks
{
    /// <summary>
    /// A class to lookup whois information.
    /// </summary>
    public static class Whois
    {
        private const int WhoisServerDefaultPortNumber = 43;

        /// <summary>
        /// Retrieves whois information
        /// </summary>
        /// <param name="domainName">The registrar or domain or name server whose whois information to be retrieved</param>
        /// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
        public static List<string> Lookup(string domainName, WhoIsRecordType recordType)
        {
            var whoisServerName = WhoisServerResolver.GetWhoisServerName(domainName);
            using (var whoisClient = new TcpClient())
            {
                whoisClient.Connect(whoisServerName, WhoisServerDefaultPortNumber);

                var domainQuery = new IdnMapping().GetAscii(domainName) + "\r\n";
                var domainQueryBytes = Encoding.ASCII.GetBytes(domainQuery.ToCharArray());

                Stream whoisStream = whoisClient.GetStream();
                whoisStream.Write(domainQueryBytes, 0, domainQueryBytes.Length);

                var whoisStreamReader = new StreamReader(whoisClient.GetStream(), Encoding.ASCII);

                string streamOutputContent;
                var whoisData = new List<string>();
                while (null != (streamOutputContent = whoisStreamReader.ReadLine()))
                {
                    whoisData.Add(streamOutputContent);
                }

                whoisClient.Close();

                return whoisData;
            }
        }
    }
}
