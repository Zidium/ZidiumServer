using System.Collections.Generic;

namespace Zidium.Api
{
    public interface IWebLogQueue
    {
        object SynchRoot { get; }

        void AddRange(List<WebLogMessage> messages);

        void Add(WebLogMessage message);

        List<WebLogMessage> GetBatchMessages(int batchBytes);

        int Count();

        int Bytes();

        void ClearBySize(int queueBytes);
    }
}
