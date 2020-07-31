using System;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public interface IEventCacheReadObject : IAccountDbCacheReadObject
    {
        /// <summary>
        /// ИД владельца. Например, для события проверки - это ИД проверки.
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// Ссылка на тип события
        /// </summary>
        Guid EventTypeId { get; }

        /// <summary>
        /// Сообщение
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Важность события
        /// </summary>
        EventImportance Importance { get; }

        /// <summary>
        /// Предыдущая важность. Используется для событий статусов.
        /// </summary>
        EventImportance PreviousImportance { get; }

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Хэш ключа по которому склеиваются события
        /// </summary>
        long JoinKeyHash { get; }

        /// <summary>
        /// Дата создания события в системе мониторинга (время системы)
        /// </summary>
        DateTime CreateDate { get; }

        /// <summary>
        /// Дата начала события (по местному времени аккаунта)
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        DateTime EndDate { get; }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        DateTime ActualDate { get; }

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        bool IsSpace { get; }

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        DateTime LastUpdateDate { get; }

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        DateTime? LastNotificationDate { get; }

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        bool IsUserHandled { get; }

        /// <summary>
        /// Версия компонента на момент появления события
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Версия в виде числа для сравнений
        /// </summary>
        long? VersionLong { get; }

        /// <summary>
        /// Категория события
        /// </summary>
        EventCategory Category { get; }

        /// <summary>
        /// Длительность события
        /// </summary>
        TimeSpan Duration { get; }

        TimeSpan GetDuration(DateTime now);

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        Guid? LastStatusEventId { get; }

        /// <summary>
        /// Ссыдка на первую причину статуса
        /// </summary>
        Guid? FirstReasonEventId { get; }

        TimeSpan GetActualInterval();
    }
}
