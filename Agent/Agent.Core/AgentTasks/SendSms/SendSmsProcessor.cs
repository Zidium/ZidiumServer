using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.SendSms
{
    public class SendSmsProcessor
    {
        public string ApiId { get; set; }

        public string From { get; set; }

        public bool FakeMode { get; set; }

        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        public SendSmsProcessor(
            ILogger logger, CancellationToken cancellationToken,
            string apiId,
            string from)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            ApiId = apiId;
            From = from;
        }

        public string Process(Guid? smsId = null)
        {
            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var smss = storage.SendSmsCommands.GetForSend(SendMaxCount);

            if (smsId.HasValue)
                smss = smss.Where(t => t.Id == smsId.Value).ToArray();

            var count = smss.Length;
            if (count > 0)
                Logger.Debug("Всего sms для отправки: " + count);
            else
                Logger.Trace("Нет sms для отправки");

            string error = null;

            foreach (var sms in smss)
            {
                CancellationToken.ThrowIfCancellationRequested();

                Logger.Debug("Отправка sms на номер {0}", sms.Phone);
                try
                {
                    // отправим сообщение
                    var externalId = SendSms(sms.Phone, sms.Body);

                    // отправим событие = чтобы знать, сколько sms в день отправлено
                    /*
                    DbProcesor.ComponentControl
                        .CreateComponentEvent("Отправка sms")
                        .SetJoinInterval(TimeSpan.FromDays(1))
                        .SetImportance(EventImportance.Unknown)
                        .SetJoinKey(DateTime.Now.ToString("yyyy.MM.dd"))
                        .Add();
                    */
                    // TODO Тут нужно писать метрику

                    // обновим статус  и статистику
                    storage.SendSmsCommands.MarkAsSendSuccessed(sms.Id, DateTime.Now, externalId);
                    Interlocked.Increment(ref SuccessSendCount);

                    Logger.Info("Отправлено sms на номер {0}", sms.Phone);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidPhoneException exception)
                {
                    // О неправильном номере телефона запишем в лог
                    // Это не ошибка отправщика

                    Logger.Info(exception.Message, new { SmsId = sms.Id.ToString() });

                    storage.SendSmsCommands.MarkAsSendFail(sms.Id, DateTime.Now, exception.Message);
                }
                catch (Exception exception)
                {
                    error = exception.Message;

                    exception.Data.Add("SmsId", sms.Id);
                    Logger.Error(exception);

                    storage.SendSmsCommands.MarkAsSendFail(sms.Id, DateTime.Now, exception.Message);
                }
            }

            return error;
        }

        public string SendSms(string phone, string body)
        {
            try
            {
                phone = PhoneHelper.NormalizePhone(phone);
            }
            catch (Exception e)
            {
                throw new InvalidPhoneException(e);
            }

            if (!FakeMode)
            {
                var url = "http://sms.ru/sms/send";
                var parameters = string.Format("api_id={0}&to={1}&text={2}&from={3}", ApiId, phone, body, From);

                var request = WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                var bytes = Encoding.UTF8.GetBytes(parameters);
                request.ContentLength = bytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                string answer;
                using (var response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        answer = streamReader.ReadToEnd().Trim();
                    }
                }

                var lines = answer.Split(new[] { "\n" }, StringSplitOptions.None);
                var resultCode = Convert.ToInt32(lines[0]);

                if (resultCode != 100)
                {
                    throw new SendSmsException(resultCode);
                }

                var externalId = lines[1];

                return externalId;
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
