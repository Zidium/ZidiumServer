using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Проверка
    /// </summary>
    public class UnitTestForUpdate
    {
        public UnitTestForUpdate(Guid id)
        {
            Id = id;
            ComponentId = new ChangeTracker<Guid>();
            SystemName = new ChangeTracker<string>();
            DisplayName = new ChangeTracker<string>();
            PeriodSeconds = new ChangeTracker<int?>();
            StatusDataId = new ChangeTracker<Guid>();
            NextExecutionDate = new ChangeTracker<DateTime?>();
            NextStepProcessDate = new ChangeTracker<DateTime?>();
            LastExecutionDate = new ChangeTracker<DateTime?>();
            DisableToDate = new ChangeTracker<DateTime?>();
            DisableComment = new ChangeTracker<string>();
            Enable = new ChangeTracker<bool>();
            ParentEnable = new ChangeTracker<bool>();
            SimpleMode = new ChangeTracker<bool>();
            IsDeleted = new ChangeTracker<bool>();
            ErrorColor = new ChangeTracker<UnitTestResult?>();
            NoSignalColor = new ChangeTracker<ObjectColor?>();
            ActualTimeSecs = new ChangeTracker<int?>();
            AttempCount = new ChangeTracker<int>();
            AttempMax = new ChangeTracker<int>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        public ChangeTracker<Guid> ComponentId { get; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Период выполнения. Используется для проверок, которые запускает через
        /// определенный интервал сам Зидиум (например проверка открытия страницы или пинг)
        /// </summary>
        public ChangeTracker<int?> PeriodSeconds { get; }

        /// <summary>
        /// Ссылка на данные статуса
        /// </summary>
        public ChangeTracker<Guid> StatusDataId { get; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public ChangeTracker<DateTime?> NextExecutionDate { get; }

        public ChangeTracker<DateTime?> NextStepProcessDate { get; }

        /// <summary>
        /// Время последнего выполнения
        /// </summary>
        public ChangeTracker<DateTime?> LastExecutionDate { get; }

        /// <summary>
        /// Дата, до которой выключена проверка
        /// </summary>
        public ChangeTracker<DateTime?> DisableToDate { get; }

        /// <summary>
        /// Комментарий к выключению (указывается при выключении)
        /// </summary>
        public ChangeTracker<string> DisableComment { get; }

        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        public ChangeTracker<bool> Enable { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public ChangeTracker<bool> ParentEnable { get; }

        /// <summary>
        /// Признак редактирования в упрощённом режиме в ЛК
        /// </summary>
        public ChangeTracker<bool> SimpleMode { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае провала проверки
        /// </summary>
        public ChangeTracker<UnitTestResult?> ErrorColor { get; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ChangeTracker<ObjectColor?> NoSignalColor { get; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public ChangeTracker<int?> ActualTimeSecs { get; }

        /// <summary>
        /// Количество выполненных неуспешных попыток
        /// </summary>
        public ChangeTracker<int> AttempCount { get; }

        /// <summary>
        /// Максимальное количество неуспешных попыток, после которого проверка провалится
        /// </summary>
        public ChangeTracker<int> AttempMax { get; }        
        
    }
}