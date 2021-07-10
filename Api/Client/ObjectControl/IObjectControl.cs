using System;

namespace Zidium.Api
{
    public interface IObjectControl
    {
        IClient Client { get; }

        string SystemName { get; }

        /// <summary>
        /// True - если программе не удалось получить данные объекта от сервера.
        /// Например, если нет интернета.
        /// </summary>
        /// <returns></returns>
        bool IsFake();
    }
}
