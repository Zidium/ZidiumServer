using System.Collections.Generic;

namespace Zidium.Api
{
    public interface IEventQueue
    {
        object SynchRoot { get; }

        void Add(BufferEventData bufferEvent);

        List<BufferEventData> GetAllByGlobalJoinKey(long globalJoinKey);

        int SizeBytes { get; }

        int MaxSizeBytes { get; set; }

        int Count();

        List<BufferEventData> GetAll();
        
        void UpdateSize(BufferEventData bufferEvent);
    }
}
