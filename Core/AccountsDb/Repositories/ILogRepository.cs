using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Core
{
    /// <summary>
    /// Репозиторий для работы с логами
    /// </summary>
    public interface ILogRepository : IComponentBasedRepository<Log>
    {
        /// <summary>
        /// Добавляет много записей в лог
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        List<Log> AddLogs(List<Log> logs);

        /// <summary>
        /// Выполняет поиск записей в логе
        /// </summary>
        /// <param name="maxCount">Максимальное количество записей в результате поиска</param>
        /// <param name="componentId">ИД компонента</param>
        /// <param name="fromDate">Поиск будет выполняется с указанной даты (ВКЛЮЧИТЕЛЬНО)</param>
        /// <param name="toDate">Поиск будет выполняется до указанной даты (НЕ ВКЛЮЧИТЕЛЬНО)</param>
        /// <param name="importanceLevels">Уровни важности сообщений. Если список пустой или null, по фильтрация по уровню не выполняется.</param>
        /// <param name="context">Контекст</param>
        /// <returns>записи лога, удовлетворяющие поисковому запросу</returns>
        List<Log> Find(
            int maxCount, 
            Guid componentId, 
            DateTime? fromDate, 
            DateTime? toDate, 
            IEnumerable<LogLevel> importanceLevels,
            string context);

        IQueryable<Log> GetFirstRecords(
            Guid componentId,
            DateTime? fromDate,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        IQueryable<Log> GetLastRecords(
            Guid componentId,
            DateTime? toDate,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        IQueryable<Log> GetPreviousRecords(
            Guid componentId,
            DateTime date, 
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        IQueryable<Log> GetNextRecords(
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        LogSearchResult FindPreviousRecordByText(
            string text,
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        LogSearchResult FindNextRecordByText(
            string text,
            Guid componentId,
            DateTime date,
            int order,
            LogLevel[] importanceLevels,
            string context,
            int maxCount);

        int DeleteLogProperties(int maxCount, DateTime toDate);

        int DeleteLogs(int maxCount, DateTime toDate);

        IQueryable<Log> QueryAll(Guid[] componentIds);
        
    }

    /// <summary>
    /// Результат поиска по тексту
    /// </summary>
    public class LogSearchResult
    {
        /// <summary>
        /// Удалось найти?
        /// </summary>
        public bool Found { get; set; }

        /// <summary>
        /// Запись лога, которую удалось найти, или с которой можно продолжить поиск
        /// </summary>
        public Log Record { get; set; }
    }
}
