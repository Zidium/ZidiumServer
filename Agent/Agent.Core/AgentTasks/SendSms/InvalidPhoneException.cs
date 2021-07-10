using System;

namespace Zidium.Agent.AgentTasks.SendSms
{
    public class InvalidPhoneException : Exception
    {
        public InvalidPhoneException(Exception innerException) : base(innerException.Message, innerException) { }
    }
}
