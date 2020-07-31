using System;

namespace Zidium.Storage
{
    public class UnitTestForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Ссылка на тип проверки
        /// </summary>
        public Guid TypeId;

        /// <summary>
        /// Период выполнения. Используется для проверок, которые запускает через
        /// определенный интервал сам Зидиум (например проверка открытия страницы или пинг)
        /// </summary>
        public int? PeriodSeconds;

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId;

        /// <summary>
        /// Ссылка на данные статуса
        /// </summary>
        public Guid StatusDataId;

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public DateTime? NextExecutionDate;

        public DateTime? NextStepProcessDate;

        /// <summary>
        /// Время последнего выполнения
        /// </summary>
        public DateTime? LastExecutionDate;

        /// <summary>
        /// Дата, до которой выключена проверка
        /// </summary>
        public DateTime? DisableToDate;

        /// <summary>
        /// Комментарий к выключению (указывается при выключении)
        /// </summary>
        public string DisableComment;

        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        public bool Enable;

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable;

        /// <summary>
        /// Признак редактирования в упрощённом режиме в ЛК
        /// </summary>
        public bool SimpleMode;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае провала проверки
        /// </summary>
        public UnitTestResult? ErrorColor;

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor;

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs;

        /// <summary>
        /// Количество выполненных неуспешных попыток
        /// </summary>
        public int AttempCount;

        /// <summary>
        /// Максимальное количество неуспешных попыток, после которого проверка провалится
        /// </summary>
        public int AttempMax;
    }
}
