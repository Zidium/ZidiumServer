using System;

namespace Zidium.Storage
{
    public interface ILogRepository
    {
        void Add(LogForAdd entity);

        void Add(LogForAdd[] entities);

        LogForRead GetOneById(Guid id);

        LogForRead[] Find(
            int maxCount,
            Guid componentId,
            DateTime? fromDate,
            DateTime? toDate,
            LogLevel[] importanceLevels,
            string context,
            string message,
            string propertyName,
            string propertyValue);

        int DeleteLogProperties(int maxCount, DateTime toDate);

        int DeleteLogs(int maxCount, DateTime toDate);

        LogForRead[] GetFirstRecords(
            Guid componentId, 
            DateTime? fromDate, 
            LogLevel[] importanceLevels,
            string context, 
            int maxCount);

        LogForRead[] GetLastRecords(
            Guid componentId, 
            DateTime? toDate, 
            LogLevel[] importanceLevels, 
            string context, 
            int maxCount);

        LogForRead[] GetPreviousRecords(
            Guid componentId, 
            DateTime date, 
            int order, 
            LogLevel[] importanceLevels, 
            string context, 
            int maxCount);

        LogForRead[] GetNextRecords(
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


    }
}
