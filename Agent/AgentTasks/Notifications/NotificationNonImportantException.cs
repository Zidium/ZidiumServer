using System;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// При отправке уведомления возникла ошибка, которая не считается важной
    /// </summary>
    class NotificationNonImportantException : Exception
    {
        public NotificationNonImportantException(Exception innerException)
            : base(null, innerException)
        {

        }
    }
}
