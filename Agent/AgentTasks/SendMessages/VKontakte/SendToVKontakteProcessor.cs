using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Api.Dto;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public class SendToVKontakteProcessor : SendMessagesProcessorBase
    {
        public SendToVKontakteProcessor(ILogger logger, CancellationToken cancellationToken, string authToken) : base(logger, cancellationToken)
        {
            AuthToken = authToken;
            _serializer = new JsonSerializer();
        }

        private readonly JsonSerializer _serializer;

        public string AuthToken { get; set; }

        protected override SubscriptionChannel Channel { get; } = SubscriptionChannel.VKontakte;

        protected override void Send(string to, string body, ForEachAccountData data)
        {
            if (FakeMode)
                return;

            var parameters = new Dictionary<string, string>();
            parameters.Add("access_token", AuthToken);
            parameters.Add("v", "5.90");
            parameters.Add("user_id", to);
            parameters.Add("random_id", new Random().Next(int.MaxValue).ToString());
            parameters.Add("message", body);
            parameters.Add("dont_parse_links", "1");
            var postdata = string.Join("&", parameters.Select(t => t.Key + "=" + HttpUtility.UrlEncode(t.Value)));

            var url = "https://api.vk.com/method/messages.send";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.UserAgent = "Zidium";

            var requestBytes = Encoding.UTF8.GetBytes(postdata);
            httpRequest.ContentLength = requestBytes.Length;

            try
            {
                using (var stream = httpRequest.GetRequestStream())
                {
                    stream.Write(requestBytes, 0, requestBytes.Length);
                    stream.Close();
                }
            }
            catch (WebException exception)
            {
                data.Logger.Error(exception);

                // Отменяем обработку до следующего запуска
                throw new OperationCanceledException();
            }

            try
            {
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    using (var responseStream = httpResponse.GetResponseStream())
                    {
                        using (var binaryReader = new BinaryReader(responseStream))
                        {
                            var responseBytes = binaryReader.ReadBytes((int)httpResponse.ContentLength);
                            var responseData = _serializer.GetObject<VKontakteSendMessageResponse>(responseBytes);

                            if (responseData.error != null)
                            {
                                if (responseData.error.error_code == 6 /*Too many requests per second*/)
                                    throw new SendMessagesOverlimitException(
                                        responseData.error.error_code + ": " + responseData.error.error_msg);

                                if (responseData.error.error_code == 901 /*Can't send messages for users without permission*/)
                                    throw new SendMessagesDisabledException(responseData.error.error_code + ": " + responseData.error.error_msg);

                                throw new Exception(responseData.error.error_code + ": " + responseData.error.error_msg);
                            }
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                data.Logger.Error(exception);

                // Отменяем обработку до следующего запуска
                throw new OperationCanceledException();
            }
        }

        public class VKontakteSendMessageResponse
        {
            public long response;

            public VKontakteErrorResponse error;
        }

        public class VKontakteErrorResponse
        {
            public long error_code;

            public string error_msg;
        }
    }
}