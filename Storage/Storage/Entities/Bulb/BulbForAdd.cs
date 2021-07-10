using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class BulbForAdd
    {
        public Guid Id;

        public Guid? ComponentId;

        public Guid? UnitTestId;

        public Guid? MetricId;

        public EventCategory EventCategory;

        public MonitoringStatus Status;

        public MonitoringStatus PreviousStatus;

        /// <summary>
        /// Дата создания статуса
        /// CreateDate >= StartDate
        /// Если создаем пробел, то начало пробела = конец предыдущего статуса, с дата создания = текущее время
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Время начала статуса
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        public DateTime EndDate;

        public int Count;

        public DateTime ActualDate;

        public string Message;

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        public Guid? FirstEventId;

        /// <summary>
        /// Последнее событие, которое привело к данному статусу (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        public Guid? LastEventId;

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        public Guid? LastChildBulbId;

        //public virtual StatusData LastChildStatus;

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        public Guid StatusEventId;

        /// <summary>
        /// Время начала расчета UpTime
        /// </summary>
        public DateTime UpTimeStartDate;

        /// <summary>
        /// Длина статистики UpTime в мс
        /// </summary>
        public long UpTimeLengthMs;

        /// <summary>
        /// Количество исправной работы в мс
        /// </summary>
        public long UpTimeSuccessMs;

        /// <summary>
        /// Предыдущий сигнал просрочен
        /// </summary>
        public bool HasSignal;

        public bool IsDeleted;
    }
}
