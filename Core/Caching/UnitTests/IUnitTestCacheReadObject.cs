using System;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core.Caching
{
    public interface IUnitTestCacheReadObject : IAccountDbCacheReadObject
    {
        string SystemName { get; }

        /// <summary>
        /// Название
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Ссылка на тип юнит-теста
        /// </summary>
        Guid TypeId { get; }

        /// <summary>
        /// Период выполнения. Используется для юнит-тестов, которые запускает через определенный интервал сам АПП (например проверка открытия страницы или пинг)
        /// </summary>
        int? PeriodSeconds { get; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        Guid ComponentId { get; }

        Guid StatusDataId { get; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        DateTime? NextDate { get; }

        /// <summary>
        /// Дата до которой выключен юнит-тест
        /// </summary>
        DateTime? DisableToDate { get; }

        /// <summary>
        /// Комментарий к юнит-тесту (указывается при выключении)
        /// </summary>
        string DisableComment { get; }

        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        bool ParentEnable { get; }

        /// <summary>
        /// Проверка и компонент включены
        /// </summary>
        bool CanProcess { get; }

        bool SimpleMode { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; }

        DateTime CreateDate { get; }

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае НЕ прохождения теста
        /// </summary>
        UnitTestResult? ErrorColor { get; }

        ObjectColor? NoSignalColor { get; }

        TimeSpan? ActualTime { get; }

        /// <summary>
        /// Проверяет что тип проверки системный
        /// </summary>
        bool IsSystemType { get; }

        int AttempCount { get; }

        int AttempMax { get; }
    }
}
