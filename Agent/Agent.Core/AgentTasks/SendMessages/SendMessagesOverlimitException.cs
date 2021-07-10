using System;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public class SendMessagesOverlimitException : Exception
    {
        public SendMessagesOverlimitException(string message) : base(message)
        {
        }
    }
}
