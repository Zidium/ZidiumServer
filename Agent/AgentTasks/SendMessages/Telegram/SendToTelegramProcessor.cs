using System;
using System.IO;
using System.Net;
using System.Threading;
using NLog;
using Zidium.Api.Dto;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public class SendToTelegramProcessor : SendMessagesProcessorBase
    {
        public SendToTelegramProcessor(ILogger logger, CancellationToken cancellationToken, string botToken) : base(logger, cancellationToken)
        {
            BotToken = botToken;
            _serializer = new JsonSerializer();
        }

        private readonly JsonSerializer _serializer;

        public string BotToken { get; set; }

        protected override SubscriptionChannel Channel { get; } = SubscriptionChannel.Telegram;

        protected override void Send(string to, string body, ForEachAccountData data)
        {
            if (FakeMode)
                return;

            var requestObj = new TelegramSendMessageRequest()
            {
                chat_id = to,
                text = body
            };

            var url = $"https://api.telegram.org/bot{BotToken}/sendMessage";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            httpRequest.UserAgent = "Zidium";

            var requestBytes = _serializer.GetBytes(requestObj);
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
                            var responseData = _serializer.GetObject<TelegramSendMessageResponse>(responseBytes);

                            if (!responseData.ok)
                            {
                                throw new Exception(responseData.error_code + ": " + responseData.description);
                            }
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                using (var response = exception.Response)
                {
                    var httpResponse = (HttpWebResponse)response;

                    using (var responseStream = httpResponse.GetResponseStream())
                    {
                        using (var binaryReader = new BinaryReader(responseStream))
                        {
                            var responseBytes = binaryReader.ReadBytes((int)httpResponse.ContentLength);
                            var responseData = _serializer.GetObject<TelegramSendMessageResponse>(responseBytes);

                            if (responseData.error_code == 429 /*Too many requests*/)
                                throw new SendMessagesOverlimitException(responseData.error_code + ": " + responseData.description);

                            if (responseData.error_code == 400 /*Bad request*/ && responseData.description.IndexOf("chat not found", StringComparison.OrdinalIgnoreCase) >= 0)
                                throw new SendMessagesDisabledException(responseData.error_code + ": " + responseData.description);
                        }
                    }
                }

                data.Logger.Error(exception);

                // Отменяем обработку до следующего запуска
                throw new OperationCanceledException();
            }
        }

        public class TelegramSendMessageRequest
        {
            public string chat_id;

            public string text;
        }

        public class TelegramSendMessageResponse
        {
            public bool ok;

            public long error_code;

            public string description;
        }
    }
}