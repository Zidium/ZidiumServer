using System;

namespace Zidium.Agent.AgentTasks.SendSms
{
    public class SendSmsException : Exception
    {
        protected int ResultCode;

        public SendSmsException(int resultCode)
        {
            ResultCode = resultCode;
        }

        public override string Message
        {
            get
            {
                return "Не удалось отправить sms, код результата: " + ResultCode;
            }
        }
    }
}
