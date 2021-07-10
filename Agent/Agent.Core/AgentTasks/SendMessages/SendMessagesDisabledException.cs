using System;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public class SendMessagesDisabledException : Exception
    {
        public SendMessagesDisabledException(string message) : base(message)
        {
        }
    }
}
