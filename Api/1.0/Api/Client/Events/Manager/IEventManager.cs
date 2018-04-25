namespace Zidium.Api
{
    public interface IEventManager
    {
        AddEventResult AddEvent(SendEventBase eventBase);
        
        void Flush();

        void Start();

        void Stop();

        long GetQueueSize();
    }
}
