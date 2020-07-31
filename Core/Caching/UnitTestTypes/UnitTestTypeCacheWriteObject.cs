﻿using System;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class UnitTestTypeCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject>, IUnitTestTypeCacheReadObject
    {
        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Ссылка на аккаунт
        /// </summary>
        public Guid AccountId { get; set; }

        public DateTime? CreateDate { get; private set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        public bool IsSystem { get; private set; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; set; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs { get; set; }

        public static UnitTestTypeCacheWriteObject Create(UnitTestTypeForRead unitTestType, Guid accountId)
        {
            if (unitTestType == null)
            {
                return null;
            }
            var cache = new UnitTestTypeCacheWriteObject()
            {
                AccountId = accountId,
                Id = unitTestType.Id,
                IsDeleted = unitTestType.IsDeleted,
                CreateDate = unitTestType.CreateDate,
                SystemName = unitTestType.SystemName,
                DisplayName = unitTestType.DisplayName,
                NoSignalColor = unitTestType.NoSignalColor,
                ActualTimeSecs = unitTestType.ActualTimeSecs,
                IsSystem = unitTestType.IsSystem
            };
            return cache;
        }

        public override int GetCacheSize()
        {
            return 200;
        }

        public UnitTestTypeForUpdate CreateEf()
        {
            return new UnitTestTypeForUpdate(Id);
        }
    }
}
