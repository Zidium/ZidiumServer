using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestRepository : IUnitTestRepository
    {
        public UnitTestRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTests.Add(new DbUnitTest()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    CreateDate = entity.CreateDate,
                    SystemName = entity.SystemName,
                    ComponentId = entity.ComponentId,
                    StatusDataId = entity.StatusDataId,
                    IsDeleted = entity.IsDeleted,
                    TypeId = entity.TypeId,
                    ActualTimeSecs = entity.ActualTimeSecs,
                    AttempCount = entity.AttempCount,
                    AttempMax = entity.AttempMax,
                    DisableComment = entity.DisableComment,
                    DisableToDate = entity.DisableToDate,
                    Enable = entity.Enable,
                    ErrorColor = entity.ErrorColor,
                    LastExecutionDate = entity.LastExecutionDate,
                    NextExecutionDate = entity.NextExecutionDate,
                    NextStepProcessDate = entity.NextStepProcessDate,
                    NoSignalColor = entity.NoSignalColor,
                    ParentEnable = entity.ParentEnable,
                    PeriodSeconds = entity.PeriodSeconds,
                    SimpleMode = entity.SimpleMode
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var unittest = DbGetOneById(entity.Id);

                if (entity.ComponentId.Changed())
                    unittest.ComponentId = entity.ComponentId.Get();

                if (entity.SystemName.Changed())
                    unittest.SystemName = entity.SystemName.Get();

                if (entity.DisplayName.Changed())
                    unittest.DisplayName = entity.DisplayName.Get();

                if (entity.PeriodSeconds.Changed())
                    unittest.PeriodSeconds = entity.PeriodSeconds.Get();

                if (entity.StatusDataId.Changed())
                    unittest.StatusDataId = entity.StatusDataId.Get();

                if (entity.NextExecutionDate.Changed())
                    unittest.NextExecutionDate = entity.NextExecutionDate.Get();

                if (entity.NextStepProcessDate.Changed())
                    unittest.NextStepProcessDate = entity.NextStepProcessDate.Get();

                if (entity.LastExecutionDate.Changed())
                    unittest.LastExecutionDate = entity.LastExecutionDate.Get();

                if (entity.DisableToDate.Changed())
                    unittest.DisableToDate = entity.DisableToDate.Get();

                if (entity.DisableComment.Changed())
                    unittest.DisableComment = entity.DisableComment.Get();

                if (entity.Enable.Changed())
                    unittest.Enable = entity.Enable.Get();

                if (entity.ParentEnable.Changed())
                    unittest.ParentEnable = entity.ParentEnable.Get();

                if (entity.SimpleMode.Changed())
                    unittest.SimpleMode = entity.SimpleMode.Get();

                if (entity.IsDeleted.Changed())
                    unittest.IsDeleted = entity.IsDeleted.Get();

                if (entity.ErrorColor.Changed())
                    unittest.ErrorColor = entity.ErrorColor.Get();

                if (entity.NoSignalColor.Changed())
                    unittest.NoSignalColor = entity.NoSignalColor.Get();

                if (entity.ActualTimeSecs.Changed())
                    unittest.ActualTimeSecs = entity.ActualTimeSecs.Get();

                if (entity.AttempCount.Changed())
                    unittest.AttempCount = entity.AttempCount.Get();

                if (entity.AttempMax.Changed())
                    unittest.AttempMax = entity.AttempMax.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var unittest = DbGetOneById(entity.Id);

                    if (entity.ComponentId.Changed())
                        unittest.ComponentId = entity.ComponentId.Get();

                    if (entity.SystemName.Changed())
                        unittest.SystemName = entity.SystemName.Get();

                    if (entity.DisplayName.Changed())
                        unittest.DisplayName = entity.DisplayName.Get();

                    if (entity.PeriodSeconds.Changed())
                        unittest.PeriodSeconds = entity.PeriodSeconds.Get();

                    if (entity.StatusDataId.Changed())
                        unittest.StatusDataId = entity.StatusDataId.Get();

                    if (entity.NextExecutionDate.Changed())
                        unittest.NextExecutionDate = entity.NextExecutionDate.Get();

                    if (entity.NextStepProcessDate.Changed())
                        unittest.NextStepProcessDate = entity.NextStepProcessDate.Get();

                    if (entity.LastExecutionDate.Changed())
                        unittest.LastExecutionDate = entity.LastExecutionDate.Get();

                    if (entity.DisableToDate.Changed())
                        unittest.DisableToDate = entity.DisableToDate.Get();

                    if (entity.DisableComment.Changed())
                        unittest.DisableComment = entity.DisableComment.Get();

                    if (entity.Enable.Changed())
                        unittest.Enable = entity.Enable.Get();

                    if (entity.ParentEnable.Changed())
                        unittest.ParentEnable = entity.ParentEnable.Get();

                    if (entity.SimpleMode.Changed())
                        unittest.SimpleMode = entity.SimpleMode.Get();

                    if (entity.IsDeleted.Changed())
                        unittest.IsDeleted = entity.IsDeleted.Get();

                    if (entity.ErrorColor.Changed())
                        unittest.ErrorColor = entity.ErrorColor.Get();

                    if (entity.NoSignalColor.Changed())
                        unittest.NoSignalColor = entity.NoSignalColor.Get();

                    if (entity.ActualTimeSecs.Changed())
                        unittest.ActualTimeSecs = entity.ActualTimeSecs.Get();

                    if (entity.AttempCount.Changed())
                        unittest.AttempCount = entity.AttempCount.Get();

                    if (entity.AttempMax.Changed())
                        unittest.AttempMax = entity.AttempMax.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public UnitTestForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public UnitTestForRead[] GetByComponentId(Guid componentId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => !t.IsDeleted && t.ComponentId == componentId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UnitTestForRead[] GetByUnitTestTypeId(Guid unitTestTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => !t.IsDeleted && t.TypeId == unitTestTypeId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public Guid[] GetNotActualIds(DateTime now, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests
                    .Where(x => !x.IsDeleted && x.Bulb.ActualDate < now)
                    .OrderBy(t => t.Bulb.ActualDate)
                    .Take(maxCount)
                    .Select(t => t.Id)
                    .ToArray();
            }
        }

        public int GetCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.Count(t => !t.IsDeleted);
            }
        }

        public UnitTestForRead[] GetForProcessing(Guid typeId, DateTime date)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(x => x.TypeId == typeId &&
                                !x.IsDeleted &&
                                x.Enable &&
                                x.ParentEnable &&
                                (x.NextExecutionDate <= date || x.NextExecutionDate == null) &&
                                (x.NextStepProcessDate <= date || x.NextStepProcessDate == null))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UnitTestForRead[] GetAllWithDeleted()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public UnitTestForRead[] Filter(
            Guid? componentTypeId,
            Guid? componentId,
            Guid? unitTestTypeId,
            MonitoringStatus[] statuses,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => !t.IsDeleted);

                if (componentTypeId.HasValue)
                    query = query.Where(t => t.Component.ComponentTypeId == componentTypeId.Value);

                if (componentId.HasValue)
                    query = query.Where(t => t.ComponentId == componentId.Value);

                if (unitTestTypeId.HasValue)
                    query = query.Where(t => t.TypeId == unitTestTypeId.Value);

                if (statuses != null && statuses.Length > 0)
                    query = query.Where(t => t.Bulb != null && statuses.Contains(t.Bulb.Status));

                query = query.OrderBy(t => t.DisplayName).Take(maxCount);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbUnitTest DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.Find(id);
            }
        }

        private DbUnitTest DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Проверка {id} не найдена");

            return result;
        }

        private UnitTestForRead DbToEntity(DbUnitTest entity)
        {
            if (entity == null)
                return null;

            return new UnitTestForRead(entity.Id, entity.SystemName, entity.DisplayName, entity.TypeId, entity.PeriodSeconds,
                entity.ComponentId, entity.StatusDataId, entity.NextExecutionDate, entity.NextStepProcessDate, entity.LastExecutionDate,
                entity.DisableToDate, entity.DisableComment, entity.Enable, entity.ParentEnable, entity.SimpleMode,
                entity.IsDeleted, entity.CreateDate, entity.ErrorColor, entity.NoSignalColor, entity.ActualTimeSecs,
                entity.AttempCount, entity.AttempMax);
        }
    }
}
