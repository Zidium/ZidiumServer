using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Тип юнит-теста
    /// </summary>
    public class DbUnitTestType
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        public string DisplayName { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; set; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs { get; set; }

    }
}
