using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public interface IBulbCacheReadObject : IAccountDbCacheReadObject
    {
        Guid? ComponentId { get; }

        Guid? UnitTestId { get;  }

        Guid? MetricId { get;  }

        EventCategory EventCategory { get;  }

        MonitoringStatus Status { get;  }

        MonitoringStatus PreviousStatus { get; }

        /// <summary>
        /// Время начала статуса
        /// </summary>
        DateTime StartDate { get;  }

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        DateTime EndDate { get;  }

        int Count { get;  }

        DateTime ActualDate { get; }

        string Message { get;  }

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        Guid? FirstEventId { get;  }

        /// <summary>
        /// Последнее событие, которое привело к данному статусу (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        Guid? LastEventId { get;  }

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        Guid? LastChildBulbId { get;  }

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        Guid StatusEventId { get; }

        /// <summary>
        /// Длительность статуса
        /// </summary>
        TimeSpan GetDuration(DateTime now);

        bool HasSignal { get; }

        bool IsSpace { get; }

        bool IsDeleted { get; }

        bool Actual(DateTime date);

        Guid GetOwnerId();

        /// <summary>
        /// Лист дерева статусов (НЕ имеет детей)
        /// </summary>
        bool IsLeaf { get; }

        Guid? GetParentId();
    }
}
