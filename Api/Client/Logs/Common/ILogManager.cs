namespace Zidium.Api
{
    public interface ILogManager
    {
        string Name { get; }

        bool Disabled { get; set; }

        /// <summary>
        /// Запись лог сообщения
        /// </summary>
        /// <param name="componentControl"></param>
        /// <param name="logMessage"></param>
        void AddLogMessage(IComponentControl componentControl, LogMessage logMessage);

        long GetQueueSize();

        int GetQueueCount();

        void Flush();

        void Start();

        void Stop();
    }
}
