using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.SendSms
{
    public class SendSmsProcessor
    {
        public string ApiId { get; set; }

        public string From { get; set; }

        public bool FakeMode { get; set; }

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        public SendSmsProcessor(
            ILogger logger, CancellationToken cancellationToken,
            string apiId,
            string from)
        {
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
            ApiId = apiId;
            From = from;
        }

        public void Process(Guid? accountId = null, Guid? smsId = null)
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (accountId == null)
                    ProcessAccount(data);
                else if (accountId == data.Account.Id)
                    ProcessAccount(data, smsId);
            });
        }

        protected void ProcessAccount(ForEachAccountData data, Guid? smsId = null)
        {
            var account = data.Account;

            var repository = data.AccountDbContext.GetSendSmsCommandRepository();
            var smss = repository.GetForSend(SendMaxCount);

            if (smsId.HasValue)
                smss = smss.Where(t => t.Id == smsId.Value).ToList();

            var count = smss.Count;
            if (count > 0)
                data.Logger.Debug("Всего sms для отправки: " + count);
            else
                data.Logger.Trace("Нет sms для отправки");

            foreach (var sms in smss)
            {
                data.CancellationToken.ThrowIfCancellationRequested();

                data.Logger.Debug("Отправка sms на номер {0}", sms.Phone);
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
                    repository.MarkAsSendSuccessed(sms.Id, externalId);
                    Interlocked.Increment(ref SuccessSendCount);

                    data.Logger.Info("Отправлено sms на номер {0}", sms.Phone);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidPhoneException exception)
                {
                    // О неправильном номере телефона запишем в лог
                    // Это не ошибка отправщика

                    data.Logger.Info(exception.Message, new { SmsId = sms.Id.ToString() });

                    repository.MarkAsSendFail(sms.Id, exception.Message);
                }
                catch (Exception exception)
                {
                    DbProcessor.SetException(exception);

                    exception.Data.Add("SmsId", sms.Id);
                    data.Logger.Error(exception);

                    repository.MarkAsSendFail(sms.Id, exception.Message);
                }
            }
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
                    requestStream.Close();
                }

                string answer;
                using (var response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        answer = streamReader.ReadToEnd().Trim();
                    }
                }

                var lines = answer.Split(new[] {"\n"}, StringSplitOptions.None);
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
