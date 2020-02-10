using System;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Caching
{
    public class UnitTestCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject>, IUnitTestCacheReadObject
    {
        /// <summary>
        /// Id
        /// </summary>
        public new Guid Id { get; set; }

        public override int GetCacheSize()
        {
            return 16 // Id
                   + 16 // AccountId
                   + StringHelper.GetPropertySize(SystemName)
                   + StringHelper.GetPropertySize(DisplayName)
                   + 16 // TypeId
                   + 4 // PeriodSeconds
                   + 16 // ComponentId
                   + 16 // StatusDataId
                   + 8 // NextExecutionDate
                   + 8 // NextStepProcessDate
                   + 1 //Enable
                   + 1 //ParentEnable
                   + 1 //SimpleMode
                   + 1 //IsDeleted
                   + 8 //CreateDate
                   + 4; //ErrorColor
        }

        public Guid AccountId { get; set; }

        public string SystemName { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Ссылка на тип юнит-теста
        /// </summary>
        public Guid TypeId { get; set; }
        
        /// <summary>
        /// Период выполнения. Используется для юнит-тестов, которые запускает через определенный интервал сам АПП (например проверка открытия страницы или пинг)
        /// </summary>
        public int? PeriodSeconds { get; set; }
        
        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        public Guid StatusDataId { get; set; }

        /// <summary>
        /// Время следующего запуска
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        /// <summary>
        /// Время выполнения следующего шага
        /// </summary>
        public DateTime? NextStepProcessDate { get; set; }

        /// <summary>
        /// Дата до которой выключен юнит-тест
        /// </summary>
        public DateTime? DisableToDate { get; set; }

        /// <summary>
        /// Комментарий к юнит-тесту (указывается при выключении)
        /// </summary>
        public string DisableComment { get; set; }
        
        /// <summary>
        /// Признак что проверка включена
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; set; }

        /// <summary>
        /// Проверка и компонент включены
        /// </summary>
        public bool CanProcess
        {
            get { return Enable && ParentEnable; }
        }

        public bool SimpleMode { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Переопределение цвета ошибки
        /// Это цвет, который будет у ошибки в случае НЕ прохождения теста
        /// </summary>
        public UnitTestResult? ErrorColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public TimeSpan? ActualTime { get; set; }

        public DateTime? LastExecutionDate { get; set; }

        /// <summary>
        /// Проверяет что тип проверки системный
        /// </summary>
        public bool IsSystemType
        {
            get { return SystemUnitTestTypes.IsSystem(TypeId); }
        }

        public int AttempCount { get; set; }

        public int AttempMax { get; set; }

        public static UnitTestCacheWriteObject Create(UnitTest unitTest, Guid accountId)
        {
            if (unitTest == null)
            {
                return null;
            }

            return new UnitTestCacheWriteObject()
            {
                Id = unitTest.Id,
                AccountId = accountId,
                ErrorColor = unitTest.ErrorColor,
                Enable = unitTest.Enable,
                DisableComment = unitTest.DisableComment,
                DisableToDate = unitTest.DisableToDate,
                DisplayName = unitTest.DisplayName,
                CreateDate = unitTest.CreateDate,
                ComponentId = unitTest.ComponentId,
                IsDeleted = unitTest.IsDeleted,
                NextExecutionDate = unitTest.NextExecutionDate,
                NextStepProcessDate = unitTest.NextStepProcessDate,
                ParentEnable = unitTest.ParentEnable,
                PeriodSeconds = unitTest.PeriodSeconds,
                SimpleMode = unitTest.SimpleMode,
                StatusDataId = unitTest.StatusDataId,
                SystemName = unitTest.SystemName,
                TypeId = unitTest.TypeId,
                NoSignalColor = unitTest.NoSignalColor,
                ActualTime = TimeSpanHelper.FromSeconds(unitTest.ActualTimeSecs),
                LastExecutionDate = unitTest.LastExecutionDate,
                AttempCount = unitTest.AttempCount,
                AttempMax = unitTest.AttempMax
            };
        }

        public UnitTest CreateEf()
        {
            return new UnitTest()
            {
                Id = Id,
                ErrorColor = ErrorColor,
                Enable = Enable,
                DisplayName = DisplayName,
                DisableToDate = DisableToDate,
                DisableComment = DisableComment,
                CreateDate = CreateDate,
                ComponentId = ComponentId,
                IsDeleted = IsDeleted,
                NextExecutionDate = NextExecutionDate,
                NextStepProcessDate = NextStepProcessDate,
                ParentEnable = ParentEnable,
                PeriodSeconds = PeriodSeconds,
                SimpleMode = SimpleMode,
                StatusDataId = StatusDataId,
                SystemName = SystemName,
                TypeId = TypeId,
                NoSignalColor = NoSignalColor,
                ActualTimeSecs = TimeSpanHelper.GetSeconds(ActualTime),
                LastExecutionDate = LastExecutionDate,
                AttempCount = AttempCount,
                AttempMax = AttempMax
            };
        }
    }
}
