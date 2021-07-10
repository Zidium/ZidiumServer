using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип проверки
    /// </summary>
    public class UnitTestTypeForUpdate
    {
        public UnitTestTypeForUpdate(Guid id)
        {
            Id = id;
            SystemName = new ChangeTracker<string>();
            DisplayName = new ChangeTracker<string>();
            IsDeleted = new ChangeTracker<bool>();
            NoSignalColor = new ChangeTracker<ObjectColor?>();
            ActualTimeSecs = new ChangeTracker<int?>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ChangeTracker<ObjectColor?> NoSignalColor { get; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public ChangeTracker<int?> ActualTimeSecs { get; }

    }
}