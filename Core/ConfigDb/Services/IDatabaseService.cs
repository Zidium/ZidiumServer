using System;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Создание новой базы
        /// </summary>
        Guid CreateDatabase(string databaseName);

        /// <summary>
        /// Получение списка всех баз
        /// </summary>
        DatabaseInfo[] GetDatabases();

        /// <summary>
        /// Получение базы по Id, если она есть
        /// </summary>
        DatabaseInfo GetOneOrNullById(Guid databaseId);

        /// <summary>
        /// Получение базы по Id
        /// </summary>
        DatabaseInfo GetOneById(Guid databaseId);
        
        /// <summary>
        /// Изменение признака неработоспособности базы
        /// </summary>
        void SetIsBroken(Guid id, bool isBroken);
    }
}
