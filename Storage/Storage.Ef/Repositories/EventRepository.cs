using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class EventRepository : IEventRepository
    {
        public EventRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(EventForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Events.Add(new DbEvent()
                {
                    Id = entity.Id,
                    CreateDate = entity.CreateDate,
                    ActualDate = entity.ActualDate,
                    Category = entity.Category,
                    Count = entity.Count,
                    EndDate = entity.EndDate,
                    EventTypeId = entity.EventTypeId,
                    FirstReasonEventId = entity.FirstReasonEventId,
                    Importance = entity.Importance,
                    IsSpace = entity.IsSpace,
                    IsUserHandled = entity.IsUserHandled,
                    JoinKeyHash = entity.JoinKeyHash,
                    LastNotificationDate = entity.LastNotificationDate,
                    LastStatusEventId = entity.LastStatusEventId,
                    LastUpdateDate = entity.LastUpdateDate,
                    Message = entity.Message,
                    OwnerId = entity.OwnerId,
                    PreviousImportance = entity.PreviousImportance,
                    StartDate = entity.StartDate,
                    Version = entity.Version,
                    VersionLong = entity.VersionLong,
                    Properties = entity.Properties?.Select(t => new DbEventProperty()
                    {
                        Id = t.Id,
                        EventId = t.EventId,
                        DataType = t.DataType,
                        Name = t.Name,
                        Value = t.Value
                    }).ToArray()
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Add(EventForAdd[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    contextWrapper.Context.Events.Add(new DbEvent()
                    {
                        Id = entity.Id,
                        CreateDate = entity.CreateDate,
                        ActualDate = entity.ActualDate,
                        Category = entity.Category,
                        Count = entity.Count,
                        EndDate = entity.EndDate,
                        EventTypeId = entity.EventTypeId,
                        FirstReasonEventId = entity.FirstReasonEventId,
                        Importance = entity.Importance,
                        IsSpace = entity.IsSpace,
                        IsUserHandled = entity.IsUserHandled,
                        JoinKeyHash = entity.JoinKeyHash,
                        LastNotificationDate = entity.LastNotificationDate,
                        LastStatusEventId = entity.LastStatusEventId,
                        LastUpdateDate = entity.LastUpdateDate,
                        Message = entity.Message,
                        OwnerId = entity.OwnerId,
                        PreviousImportance = entity.PreviousImportance,
                        StartDate = entity.StartDate,
                        Version = entity.Version,
                        VersionLong = entity.VersionLong,
                        Properties = entity.Properties?.Select(t => new DbEventProperty()
                        {
                            Id = t.Id,
                            EventId = t.EventId,
                            DataType = t.DataType,
                            Name = t.Name,
                            Value = t.Value
                        }).ToArray()
                    });
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(EventForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var eventObj = DbGetOneById(entity.Id);

                if (entity.Count.Changed())
                    eventObj.Count = entity.Count.Get();

                if (entity.Message.Changed())
                    eventObj.Message = entity.Message.Get();

                if (entity.Importance.Changed())
                    eventObj.Importance = entity.Importance.Get();

                if (entity.EndDate.Changed())
                    eventObj.EndDate = entity.EndDate.Get();

                if (entity.ActualDate.Changed())
                    eventObj.ActualDate = entity.ActualDate.Get();

                if (entity.StartDate.Changed())
                    eventObj.StartDate = entity.StartDate.Get();

                if (entity.LastUpdateDate.Changed())
                    eventObj.LastUpdateDate = entity.LastUpdateDate.Get();

                if (entity.LastNotificationDate.Changed())
                    eventObj.LastNotificationDate = entity.LastNotificationDate.Get();

                if (entity.IsUserHandled.Changed())
                    eventObj.IsUserHandled = entity.IsUserHandled.Get();

                if (entity.LastStatusEventId.Changed())
                    eventObj.LastStatusEventId = entity.LastStatusEventId.Get();

                if (entity.FirstReasonEventId.Changed())
                    eventObj.FirstReasonEventId = entity.FirstReasonEventId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(EventForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var context = contextWrapper.Context;

                DbConnection connection = null;
                try
                {
                    connection = context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();

                        var stringBuilder = new StringBuilder();
                        var index = 0;

                        foreach (var entity in entities)
                        {
                            var setList = new List<string>();

                            if (entity.Count.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@Count" + index;
                                parameter.Value = entity.Count.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("Count") + " = @Count" + index);
                            }

                            if (entity.Message.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@Message" + index;
                                parameter.Value = (object)entity.Message.Get() ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("Message") + " = @Message" + index);
                            }

                            if (entity.Importance.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@Importance" + index;
                                parameter.Value = (int)entity.Importance.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("Importance") + " = @Importance" + index);
                            }

                            if (entity.EndDate.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@EndDate" + index;
                                parameter.Value = entity.EndDate.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("EndDate") + " = @EndDate" + index);
                            }

                            if (entity.ActualDate.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@ActualDate" + index;
                                parameter.Value = entity.ActualDate.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("ActualDate") + " = @ActualDate" + index);
                            }

                            if (entity.StartDate.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@StartDate" + index;
                                parameter.Value = entity.StartDate.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("StartDate") + " = @StartDate" + index);
                            }

                            if (entity.LastUpdateDate.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@LastUpdateDate" + index;
                                parameter.Value = entity.LastUpdateDate.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("LastUpdateDate") + " = @LastUpdateDate" + index);
                            }

                            if (entity.LastNotificationDate.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@LastNotificationDate" + index;
                                parameter.Value = (object)entity.LastNotificationDate.Get() ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("LastNotificationDate") + " = @LastNotificationDate" + index);
                            }

                            if (entity.IsUserHandled.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@IsUserHandled" + index;
                                parameter.Value = entity.IsUserHandled.Get();
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("IsUserHandled") + " = @IsUserHandled" + index);
                            }

                            if (entity.LastStatusEventId.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@LastStatusEventId" + index;
                                parameter.Value = (object)entity.LastStatusEventId.Get() ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("LastStatusEventId") + " = @LastStatusEventId" + index);
                            }

                            if (entity.FirstReasonEventId.Changed())
                            {
                                var parameter = command.CreateParameter();
                                parameter.ParameterName = "@FirstReasonEventId" + index;
                                parameter.Value = (object)entity.FirstReasonEventId.Get() ?? DBNull.Value;
                                command.Parameters.Add(parameter);
                                setList.Add(context.FormatColumnName("FirstReasonEventId") + " = @FirstReasonEventId" + index);
                            }

                            if (setList.Count > 0)
                            {
                                var idParameter = command.CreateParameter();
                                idParameter.ParameterName = "@Id" + index;
                                idParameter.Value = entity.Id;
                                command.Parameters.Add(idParameter);

                                stringBuilder.AppendLine($@"UPDATE {context.FormatTableName("Events")} SET {string.Join(", ", setList)} WHERE {context.FormatColumnName("Id")} = @Id" + index + ";");
                            }

                            index++;
                        }

                        command.CommandText = stringBuilder.ToString();

                        SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    context.ReleaseConnection(connection);
                }
            }
        }

        /*
        public void Update(EventForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var context = contextWrapper.Context;
                context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var entity in entities)
                {
                    var eventObj = new DbEvent()
                    {
                        Id = entity.Id
                    };

                    context.Events.Attach(eventObj);

                    var entry = context.Entry(eventObj);

                    if (entity.Count.Changed())
                    {
                        eventObj.Count = entity.Count.Get();
                        entry.Property(t => t.Count).IsModified = true;
                    }

                    if (entity.Message.Changed())
                    {
                        eventObj.Message = entity.Message.Get();
                        entry.Property(t => t.Message).IsModified = true;
                    }

                    if (entity.Importance.Changed())
                    {
                        eventObj.Importance = entity.Importance.Get();
                        entry.Property(t => t.Importance).IsModified = true;
                    }

                    if (entity.EndDate.Changed())
                    {
                        eventObj.EndDate = entity.EndDate.Get();
                        entry.Property(t => t.EndDate).IsModified = true;
                    }

                    if (entity.ActualDate.Changed())
                    {
                        eventObj.ActualDate = entity.ActualDate.Get();
                        entry.Property(t => t.ActualDate).IsModified = true;
                    }

                    if (entity.StartDate.Changed())
                    {
                        eventObj.StartDate = entity.StartDate.Get();
                        entry.Property(t => t.StartDate).IsModified = true;
                    }

                    if (entity.LastUpdateDate.Changed())
                    {
                        eventObj.LastUpdateDate = entity.LastUpdateDate.Get();
                        entry.Property(t => t.LastUpdateDate).IsModified = true;
                    }

                    if (entity.LastNotificationDate.Changed())
                    {
                        eventObj.LastNotificationDate = entity.LastNotificationDate.Get();
                        entry.Property(t => t.LastNotificationDate).IsModified = true;
                    }

                    if (entity.IsUserHandled.Changed())
                    {
                        eventObj.IsUserHandled = entity.IsUserHandled.Get();
                        entry.Property(t => t.IsUserHandled).IsModified = true;
                    }

                    if (entity.LastStatusEventId.Changed())
                    {
                        eventObj.LastStatusEventId = entity.LastStatusEventId.Get();
                        entry.Property(t => t.LastStatusEventId).IsModified = true;
                    }

                    if (entity.FirstReasonEventId.Changed())
                    {
                        eventObj.FirstReasonEventId = entity.FirstReasonEventId.Get();
                        entry.Property(t => t.FirstReasonEventId).IsModified = true;
                    }
                }

                contextWrapper.Context.SaveChanges();
            }
        }
        */

        public EventForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public EventForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public EventForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        // TODO Missing index
        public EventForRead GetLastEventByEndDate(Guid eventTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.EventTypeId == eventTypeId)
                    .OrderByDescending(x => x.EndDate)
                    .FirstOrDefault());
            }
        }

        public EventForRead GetForJoin(EventForAdd eventObj)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                // Первичное условие - по владельцу и типу
                // Эти поля входят в индекс
                var query = contextWrapper.Context.Events.AsNoTracking()
                    .Where(x =>
                        x.OwnerId == eventObj.OwnerId &&
                        x.EventTypeId == eventObj.EventTypeId &&
                        x.Importance == eventObj.Importance
                );

                // Вторичные условия - по остальным параметрам, у них низкая селективность
                query = query.Where(x =>
                    x.Category == eventObj.Category
                    && x.JoinKeyHash == eventObj.JoinKeyHash
                    && x.IsSpace == eventObj.IsSpace
                );

                // Версию сравниваем именно по строке
                // VersionLong заполнено только для версий формата x.x.x.x!
                if (eventObj.Version != null)
                {
                    query = query.Where(x => x.Version == eventObj.Version);
                }

                var eventId = query.OrderByDescending(x => x.ActualDate).Select(t => t.Id).FirstOrDefault();

                return eventId != Guid.Empty ? GetOneById(eventId) : null;
            }
        }

        public EventForRead[] Filter(
            Guid ownerId,
            EventCategory? category,
            DateTime? from,
            DateTime? to,
            EventImportance[] importance,
            Guid? eventTypeId,
            string searthText,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Events.AsNoTracking().Where(t => t.OwnerId == ownerId);

                if (from.HasValue)
                    query = query.Where(t => t.ActualDate >= from.Value);

                if (to.HasValue)
                    query = query.Where(t => t.StartDate < to.Value);

                if (category.HasValue)
                {
                    query = query.Where(t => t.Category == category);
                }

                if (importance != null && importance.Length > 0)
                    query = query.Where(t => importance.Contains(t.Importance));

                if (eventTypeId.HasValue)
                    query = query.Where(t => t.EventTypeId == eventTypeId.Value);

                if (string.IsNullOrEmpty(searthText) == false)
                    query = query.Where(t => t.Message.Contains(searthText));

                query = query.OrderByDescending(t => t.StartDate).Take(maxCount);

                return query.AsEnumerable().Select(DbToEntity).ToArray();
            }
        }

        public EventForRead GetMostDangerousEvent(Guid componentId, EventCategory[] categories, DateTime fromDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Events.AsNoTracking()
                    .Where(x =>
                        x.OwnerId == componentId
                        && categories.Contains(x.Category)
                        && x.StartDate <= fromDate
                        && x.ActualDate > fromDate)
                    .OrderByDescending(x => x.Importance)
                    .ThenByDescending(x => x.ActualDate) // из всех опасных возьмем самое длинное
                    .Take(1)
                    .FirstOrDefault());
            }
        }

        // TODO Missing index
        public EventForRead[] GetActualByType(Guid eventTypeId, DateTime actualDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => t.EventTypeId == eventTypeId && t.ActualDate >= actualDate)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int DeleteEventParameters(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("EventParameters")} WHERE {contextWrapper.Context.FormatColumnName("EventId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public int DeleteEvents(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"UPDATE {contextWrapper.Context.FormatTableName("Events")} SET {contextWrapper.Context.FormatColumnName("LastStatusEventId")} = NULL WHERE {contextWrapper.Context.FormatColumnName("LastStatusEventId")} IN({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        SqlCommandHelper.ExecuteNonQuery(command);
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("Events")} WHERE {contextWrapper.Context.FormatColumnName("Id")} IN({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public int DeleteEventStatuses(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                var count = 0;
                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("EventStatuses")} WHERE {contextWrapper.Context.FormatColumnName("EventId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        count += SqlCommandHelper.ExecuteNonQuery(command);
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("EventStatuses")} WHERE {contextWrapper.Context.FormatColumnName("StatusId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        count += SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }

                return count;
            }
        }

        public int UpdateMetricsHistory(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"UPDATE {contextWrapper.Context.FormatTableName("MetricHistory")} SET {contextWrapper.Context.FormatColumnName("StatusEventId")} = NULL WHERE {contextWrapper.Context.FormatColumnName("StatusEventId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public int DeleteNotifications(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var subQuery = GetEventsSubQuery(categories, toDate, maxCount);

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        var objectQuery = contextWrapper.Context.Notifications
                            .Where(t => subQuery.Contains(t.EventId)).Select(t => t.Id).ToParametrizedSql();

                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("NotificationsHttp")} WHERE {contextWrapper.Context.FormatColumnName("NotificationId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        SqlCommandHelper.ExecuteNonQuery(command);
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var objectQuery = contextWrapper.Context.Notifications
                            .Where(t => subQuery.Contains(t.EventId)).Select(t => t.Id).ToParametrizedSql();

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("LastComponentNotifications")} WHERE {contextWrapper.Context.FormatColumnName("NotificationId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        SqlCommandHelper.ExecuteNonQuery(command);
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("Notifications")} WHERE {contextWrapper.Context.FormatColumnName("EventId")} IN ({objectQuery.Sql})";

                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public int DeleteEventArchivedStatuses(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var objectQuery = GetEventsSubQuery(categories, toDate, maxCount).ToParametrizedSql();

                DbConnection connection = null;
                try
                {
                    connection = contextWrapper.Context.CreateConnection();

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = contextWrapper.Context.Database.CurrentTransaction?.GetDbTransaction();
                        command.CommandTimeout = 0;

                        var query = $"DELETE FROM {contextWrapper.Context.FormatTableName("ArchivedStatuses")} WHERE {contextWrapper.Context.FormatColumnName("EventId")} IN ({objectQuery.Sql})";
                        command.CommandText = query;

                        foreach (var objectParameter in objectQuery.Parameters)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = objectParameter.ParameterName;
                            parameter.Value = objectParameter.Value;
                            command.Parameters.Add(parameter);
                        }

                        return SqlCommandHelper.ExecuteNonQuery(command);
                    }
                }
                finally
                {
                    contextWrapper.Context.ReleaseConnection(connection);
                }
            }
        }

        public int GetEventsCountForDeletion(EventCategory[] categories, DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.Count(t => categories.Contains(t.Category) && t.ActualDate < toDate);
            }
        }

        public EventForRead GetFirstEventReason(Guid eventId)
        {
            using (_storage.GetContextWrapper())
            {
                var firstReasonEventId = DbGetOneById(eventId).FirstReasonEventId;
                return firstReasonEventId != null ? DbToEntity(DbGetOneById(firstReasonEventId.Value)) : null;
            }
        }

        public EventForRead GetRecentEventReason(Guid eventId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var recentEventReason = contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.StatusEvents.Any(y => y.Id == eventId))
                    .OrderByDescending(t => t.StartDate)
                    .FirstOrDefault();

                return DbToEntity(recentEventReason);
            }
        }

        public EventForRead[] GetEventReasons(Guid eventId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.StatusEvents.Any(y => y.Id == eventId))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] GetActualEventsWithWrongData(DateTime actualDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Bulbs.AsNoTracking()
                    .Join(contextWrapper.Context.Events.AsNoTracking(), a => a.StatusEventId, b => b.Id, (a, b) => b)
                    .Where(x => contextWrapper.Context.Events.Any(z =>
                        z.OwnerId == x.OwnerId &&
                        z.Category == x.Category &&
                        z.Id != x.Id &&
                        z.ActualDate == actualDate))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] GetEventsWithWrongData(EventForRead actualEvent, DateTime actualDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == actualEvent.OwnerId &&
                                x.Category == actualEvent.Category &&
                                x.Id != actualEvent.Id &&
                                x.ActualDate == actualDate)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(Guid ownerId, EventCategory category, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == ownerId &&
                                x.Category == category &&
                                x.StartDate <= to &&
                                x.ActualDate >= from)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(Guid ownerId, EventCategory[] categories, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == ownerId &&
                                categories.Contains(x.Category) &&
                                x.StartDate <= to &&
                                x.ActualDate >= from)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(Guid ownerId, Guid eventTypeId, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == ownerId &&
                                x.EventTypeId == eventTypeId &&
                                x.StartDate <= to &&
                                x.ActualDate >= from)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(Guid[] ownerIds, Guid eventTypeId, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => ownerIds.Contains(x.OwnerId) &&
                                x.EventTypeId == eventTypeId &&
                                x.StartDate <= to &&
                                x.ActualDate >= from)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(Guid eventTypeId, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.EventTypeId == eventTypeId &&
                                x.StartDate <= to &&
                                x.ActualDate >= from)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int GetErrorsCountByPeriod(Guid componentId, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.Count(t =>
                    t.OwnerId == componentId &&
                    t.StartDate < to &&
                    t.ActualDate >= from &&
                    t.Category == EventCategory.ApplicationError
                    );
            }
        }

        public EventForRead[] GetErrorsByPeriod(Guid? componentId, DateTime from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Events.AsNoTracking().AsQueryable();

                if (componentId.HasValue)
                    query = query.Where(t => t.OwnerId == componentId);

                query = query.Where(x =>
                    x.StartDate < to &&
                    x.ActualDate >= from &&
                    x.Category == EventCategory.ApplicationError);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] GetErrorsByPeriod(Guid[] componentIds, DateTime @from, DateTime to)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Events.AsNoTracking().AsQueryable();

                if (componentIds != null)
                    query = query.Where(t => componentIds.Contains(t.OwnerId));

                query = query.Where(x =>
                    x.StartDate < to &&
                    x.ActualDate >= from &&
                    x.Category == EventCategory.ApplicationError);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead GetLastEvent(Guid ownerId, EventCategory category)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => t.OwnerId == ownerId &&
                                t.Category == category)
                    .OrderByDescending(t => t.StartDate)
                    .FirstOrDefault());
            }
        }

        public EventForRead[] GetLastEvents(Guid ownerId, EventCategory category, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == ownerId &&
                                x.Category == category)
                    .OrderByDescending(t => t.StartDate)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        // TODO Missing index
        public EventForRead[] GetByEventTypeId(Guid eventTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => t.EventTypeId == eventTypeId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead[] Filter(
            Guid? ownerId,
            Guid? componentTypeId,
            Guid? eventTypeId,
            EventCategory[] categories,
            EventImportance[] importances,
            DateTime? fromDate,
            DateTime? toDate,
            string message,
            string versionFrom,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Events.AsNoTracking().AsQueryable();

                if (ownerId.HasValue)
                    query = query.Where(t => t.OwnerId == ownerId.Value);

                if (componentTypeId.HasValue)
                {
                    var componentIds = contextWrapper.Context.Components.AsNoTracking()
                        .Where(t => t.ComponentTypeId == componentTypeId.Value)
                        .Select(t => t.Id)
                        .ToArray();
                    query = query.Where(t => componentIds.Contains(t.OwnerId));
                }

                if (eventTypeId.HasValue)
                {
                    query = query.Where(t => t.EventTypeId == eventTypeId.Value);
                }

                if (categories != null && categories.Length > 0)
                    query = query.Where(t => categories.Contains(t.Category));

                if (importances != null && importances.Length > 0)
                    query = query.Where(t => importances.Contains(t.Importance));

                if (fromDate.HasValue)
                    query = query.Where(t => t.ActualDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.StartDate <= toDate.Value);

                if (!string.IsNullOrEmpty(message))
                    query = query.Where(t => t.Message != null && t.Message.ToLower().Contains(message.ToLower()));

                if (!string.IsNullOrEmpty(versionFrom))
                {
                    var versionFromLong = VersionHelper.FromString(versionFrom) ?? -1;
                    query = query.Where(t => (t.VersionLong ?? long.MaxValue) > versionFromLong);
                }

                query = query.OrderByDescending(t => t.StartDate).Take(maxCount);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventForRead GetByOwnerIdAndLastStatusEventId(Guid ownerId, Guid lastStatusEventId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.Events.AsNoTracking()
                    .Where(x => x.OwnerId == ownerId &&
                                x.LastStatusEventId == lastStatusEventId)
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault());
            }
        }

        public void AddStatusToEvent(EventStatusForAdd[] statuses)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var status in statuses)
                {
                    var eventObj = DbGetOneById(status.EventId);
                    var statusEventObj = DbGetOneById(status.StatusId);
                    eventObj.StatusEvents.Add(statusEventObj);
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        private DbEvent DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events.Find(id);
            }
        }

        private DbEvent DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Событие {id} не найдено");

            return result;
        }

        private EventForRead DbToEntity(DbEvent entity)
        {
            if (entity == null)
                return null;

            return new EventForRead(entity.Id, entity.OwnerId, entity.EventTypeId, entity.Message, entity.Importance,
                entity.PreviousImportance, entity.Count, entity.JoinKeyHash, entity.CreateDate, entity.StartDate,
                entity.EndDate, entity.ActualDate, entity.IsSpace, entity.LastUpdateDate, entity.LastNotificationDate,
                entity.IsUserHandled, entity.Version, entity.VersionLong, entity.Category, entity.LastStatusEventId,
                entity.FirstReasonEventId);
        }

        private IQueryable<Guid> GetEventsSubQuery(EventCategory[] categories, DateTime actualDate, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Events
                    .Where(t => categories.Contains(t.Category) && t.ActualDate < actualDate)
                    .OrderBy(t => t.Category)
                    .ThenBy(t => t.ActualDate)
                    .Select(t => t.Id)
                    .Take(maxCount);
            }
        }
    }
}
