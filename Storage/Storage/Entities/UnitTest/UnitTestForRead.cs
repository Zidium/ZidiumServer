using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Проверка
    /// </summary>
    public class UnitTestForRead
    {
        public UnitTestForRead(
            Guid id, 
            string systemName, 
            string displayName, 
            Guid typeId, 
            int? periodSeconds, 
            Guid componentId, 
            Guid statusDataId, 
            DateTime? nextExecutionDate, 
            DateTime? nextStepProcessDate, 
            DateTime? lastExecutionDate, 
            DateTime? disableToDate, 
            string disableComment, 
            bool enable, 
            bool parentEnable, 
            bool simpleMode, 
            bool isDeleted, 
            DateTime createDate, 
            UnitTestResult? errorColor, 
            ObjectColor? noSignalColor, 
            int? actualTimeSecs, 
            int attempCount, 
            int attempMax)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
            TypeId = typeId;
            PeriodSeconds = periodSeconds;
            ComponentId = componentId;
            StatusDataId = statusDataId;
            NextExecutionDate = nextExecutionDate;
            NextStepProcessDate = nextStepProcessDate;
            LastExecutionDate = lastExecutionDate;
            DisableToDate = disableToDate;
            DisableComment = disableComment;
            Enable = enable;
            ParentEnable = parentEnable;
            SimpleMode = simpleMode;
            IsDeleted = isDeleted;
            CreateDate = createDate;
            ErrorColor = errorColor;
            NoSignalColor = noSignalColor;
            ActualTimeSecs = actualTimeSecs;
            AttempCount = attempCount;
            AttempMax = attempMax;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Ссылка на тип проверки
        /// </summary>
        public Guid TypeId { get; }

        /// <summary>
        /// Период выполнения. Используется для проверок, которые запускает через
        /// определенный интервал сам Зидиум (например проверка открытия страницы или пинг)
        /// </summary>
        public int? PeriodSeconds { get; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; }

        /// <summary>
        /// Ссылка на данные статуса
        /// </summary>
        public Guid StatusDataId { get; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public DateTime? NextExecutionDate { get; }

        public DateTime? NextStepProcessDate { get; }

        /// <summary>
        /// Время последнего выполнения
        /// </summary>
        public DateTime? LastExecutionDate { get; }

        /// <summary>
        /// Дата, до которой выключена проверка
        /// </summary>
        public DateTime? DisableToDate { get; }

        /// <summary>
        /// Комментарий к выключению (указывается при выключении)
        /// </summary>
        public string DisableComment { get; }

        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; }

        /// <summary>
        /// Признак редактирования в упрощённом режиме в ЛК
        /// </summary>
        public bool SimpleMode { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае провала проверки
        /// </summary>
        public UnitTestResult? ErrorColor { get; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs { get; }

        /// <summary>
        /// Количество выполненных неуспешных попыток
        /// </summary>
        public int AttempCount { get; }

        /// <summary>
        /// Максимальное количество неуспешных попыток, после которого проверка провалится
        /// </summary>
        public int AttempMax { get; }

        public UnitTestForUpdate GetForUpdate()
        {
            return new UnitTestForUpdate(Id);
        }

    }
}
