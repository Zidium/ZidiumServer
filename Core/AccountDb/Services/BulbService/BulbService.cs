using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class BulbService : IBulbService
    {
        public BulbService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        private IEventCacheReadObject CreateOrUpdateStatusEvent(BulbCacheWriteObject data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = new AccountCacheRequest()
            {
                ObjectId = data.StatusEventId
            };

            // обновляем существующее
            if (AllCaches.Events.ExistsInStorage(request))
            {
                using (var statusEvent = AllCaches.Events.Write(request))
                {
                    statusEvent.Message = data.Message;
                    statusEvent.EndDate = data.EndDate;
                    statusEvent.StartDate = data.StartDate;
                    statusEvent.ActualDate = data.ActualDate;
                    statusEvent.Count = data.Count;
                    statusEvent.BeginSave();

                    return statusEvent;
                }
            }

            // создаем новое событие
            var ownerId = data.GetOwnerId();
            return AddStatusEvent(data, ownerId);
        }

        private IEventCacheReadObject AddStatusEvent(BulbCacheWriteObject data, Guid ownerId)
        {
            var eventType = SystemEventType.GetStatusEventType(data.EventCategory);
            var now = DateTime.Now;

            // создаем новый статус
            var newStatus = new EventForAdd()
            {
                Id = Ulid.NewUlid(),
                Message = data.Message,
                ActualDate = data.ActualDate,
                Category = data.EventCategory,
                Count = data.Count,
                CreateDate = now,
                LastUpdateDate = now,
                EndDate = data.EndDate,
                Importance = EventImportanceHelper.Get(data.Status),
                OwnerId = ownerId,
                StartDate = data.StartDate,
                EventTypeId = eventType.Id,
                PreviousImportance = EventImportanceHelper.Get(data.PreviousStatus),
                IsSpace = data.IsSpace
            };
            if (data.FirstEventId.HasValue && data.FirstEventId != Guid.Empty)
            {
                newStatus.FirstReasonEventId = data.FirstEventId;
            }

            using (var transaction = _storage.BeginTransaction())
            {
                _storage.Events.Add(newStatus);

                // обновим старый статус
                var oldStatus = _storage.Events.GetOneOrNullById(data.StatusEventId);
                if (oldStatus != null)
                {
                    // убедимся, что очереди изменений нет изменений старого статуса,
                    // иначе наши изменения через EF могут быть перезатёрты
                    using (var oldStatusCache = AllCaches.Events.GetForWrite(oldStatus.Id))
                    {
                        oldStatusCache.WaitSaveChanges();
                        oldStatusCache.Unload();

                        // установим флажок, если это архивный статус
                        if (oldStatus.Category == EventCategory.ComponentExternalStatus)
                        {
                            var archivedStatus = new ArchivedStatusForAdd()
                            {
                                EventId = oldStatus.Id
                            };
                            _storage.ArchivedStatuses.Add(archivedStatus);
                        }

                        // старый статус завершается, когда начинается новый статус
                        var oldStatusForUpdate = oldStatus.GetForUpdate();
                        oldStatusForUpdate.ActualDate.Set(newStatus.StartDate);
                        oldStatusForUpdate.EndDate.Set(newStatus.StartDate);
                        oldStatusForUpdate.LastUpdateDate.Set(now);
                        _storage.Events.Update(oldStatusForUpdate);
                    }
                }

                // обновим лампочку через EF, чтобы {лампочка, старый статус и новый статус} сохранились одновременно
                // изменения лампочки будут сохранены дважды, второй раз в очереди изменений
                // сейчас непонятно, как этого избежать, дело в том, что после данного метода код может изменить кэш лампочки
                // и, чтобы не потерять эти изменения, сохраняем дважды

                var bulbForUpdate = new BulbForUpdate(data.Id);
                bulbForUpdate.StatusEventId.Set(newStatus.Id);
                _storage.Bulbs.Update(bulbForUpdate);

                transaction.Commit();
            }
            // убедимся что в очереди изменений нет других изменений лампочки, 
            // иначе сохранённые через EF изменения могут быть потеряны

            data.StatusEventId = newStatus.Id;
            data.WaitSaveChanges();

            var rEvent = AllCaches.Events.Find(new AccountCacheRequest()
            {
                ObjectId = newStatus.Id
            });
            return rEvent;
        }

        private DateTime GetStartDate(IBulbCacheReadObject bulb, BulbSignal signal)
        {
            var startDate = signal.StartDate;

            if (signal.Status > bulb.Status)
            {
                // более важные события могут обнулять длительность предыдущих
                // обычно statusData.EndDate = statusData.StartDate
                startDate = DateTimeHelper.Max(startDate, bulb.EndDate);
            }
            else
            {
                // менее важные не могут затирать более важные
                startDate = DateTimeHelper.Max(startDate, bulb.ActualDate);
            }

            // время начала нового статуса НЕ может быть больше текущей даты
            startDate = DateTimeHelper.Min(signal.ProcessDate, startDate);

            return startDate;
        }

        private void UpdateLastStatusId(Guid eventId, IBulbCacheReadObject bulb)
        {
            if (eventId == Guid.Empty)
            {
                return;
            }
            var request = new AccountCacheRequest()
            {
                ObjectId = eventId
            };
            if (AllCaches.Events.ExistsInStorage(request))
            {
                using (var wEvent = AllCaches.Events.Write(request))
                {
                    if (wEvent.LastStatusEventId == bulb.StatusEventId)
                    {
                        return;
                    }

                    // проверим соответствие статуса и его причины
                    if (wEvent.Category == EventCategory.ComponentEventsStatus &&
                        bulb.EventCategory != EventCategory.ComponentInternalStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.ComponentEventsStatus && statusData.EventCategory!=EventCategory.ComponentInternalStatus");
                    }
                    if (wEvent.Category == EventCategory.ComponentEvent &&
                        bulb.EventCategory != EventCategory.ComponentEventsStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.ComponentEvent && statusData.EventCategory != EventCategory.ComponentEventsStatus");
                    }
                    if (wEvent.Category == EventCategory.ApplicationError &&
                        bulb.EventCategory != EventCategory.ComponentEventsStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.ApplicationError && statusData.EventCategory != EventCategory.ComponentEventsStatus");
                    }
                    if (wEvent.Category == EventCategory.ComponentInternalStatus &&
                        bulb.EventCategory != EventCategory.ComponentExternalStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.ComponentInternalStatus && statusData.EventCategory != EventCategory.ComponentExternalStatu");
                    }
                    if (wEvent.Category == EventCategory.ComponentExternalStatus &&
                        bulb.EventCategory != EventCategory.ComponentChildsStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.ComponentExternalStatus && statusData.EventCategory != EventCategory.ComponentChildsStatus");
                    }
                    if (wEvent.Category == EventCategory.UnitTestResult &&
                        bulb.EventCategory != EventCategory.UnitTestStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.UnitTestResult && statusData.EventCategory != EventCategory.UnitTestStatus");
                    }
                    if (wEvent.Category == EventCategory.UnitTestStatus &&
                        bulb.EventCategory != EventCategory.ComponentUnitTestsStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.UnitTestStatus && statusData.EventCategory != EventCategory.ComponentUnitTestsStatus");
                    }
                    if (wEvent.Category == EventCategory.MetricStatus &&
                        bulb.EventCategory != EventCategory.ComponentMetricsStatus)
                    {
                        throw new Exception("wEvent.Category == EventCategory.MetricStatus && statusData.EventCategory != EventCategory.ComponentMetricsStatus");
                    }
                    wEvent.LastStatusEventId = bulb.StatusEventId;
                    var statusRef = new EventCacheStatusReference()
                    {
                        Saved = false,
                        StatusId = bulb.StatusEventId
                    };
                    wEvent.NewStatuses.Add(statusRef);
                    wEvent.BeginSave();
                }
            }
        }

        private void ProlongStatus(BulbSignal signal, BulbCacheWriteObject data)
        {
            // проверим пробел
            var nextStartDate = GetStartDate(data, signal);
            bool hasSpace = data.ActualDate < nextStartDate;
            if (hasSpace)
            {
                throw new Exception("Тут не должно быть пробела");
            }

            // разрыва нет
            // у виртуальных колбасок нет смысла увеличивать счетчик, т.к. это приводит только к лишнему сохранению объектов
            if (data.IsLeaf)
            {
                data.Count++;
            }

            data.HasSignal = signal.HasSignal;
            data.Message = signal.Message;
            if (signal.EventId != Guid.Empty)
            {
                data.LastEventId = signal.EventId;
            }
            data.LastChildBulbId = signal.ChildBulbId;

            // у листьев время завершения меняется, 
            // а у родителей время завершения меняется только когда начинется новый статус
            if (data.IsLeaf)
            {
                // это нужно, например, чтобы у проверок и метрик EndDate показывал время последнего выполнения
                data.EndDate = signal.ProcessDate;

                // актуальность проверки и метрики определяется последним значением
                if (data.EventCategory == EventCategory.MetricStatus
                    || data.EventCategory == EventCategory.UnitTestStatus)
                {
                    data.ActualDate = signal.ActualDate;
                }
                else if (data.EventCategory == EventCategory.ComponentEventsStatus)
                {
                    if (signal.ActualDate > data.ActualDate)
                    {
                        data.ActualDate = signal.ActualDate;
                    }
                }
                else
                {
                    throw new Exception("Неизвестный тип лампочки " + data.EventCategory);
                }
            }
            else
            {
                // у виртуальных лампочек время актуальности и завершения не меняется при продлении
                data.ActualDate = DateTimeHelper.InfiniteActualDate;
            }

            CreateOrUpdateStatusEvent(data);
        }

        protected IBulbCacheReadObject ProlongOrChangeStatus(BulbSignal signal, Guid statusDataId)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = statusDataId
            };
            IBulbCacheReadObject rBulb = null;
            using (var wStatusData = AllCaches.StatusDatas.Write(request))
            {
                rBulb = wStatusData;

                // если сигнал протух, то игнорируем его
                if (signal.ActualDate < wStatusData.EndDate)
                {
                    return rBulb;
                }

                // вставим пробел
                var startTime = GetStartDate(wStatusData, signal);
                if (wStatusData.ActualDate < startTime)
                {
                    var noSignal = BulbSignal.CreateNoSignal(wStatusData.ActualDate, signal.NoSignalImportance);
                    ChangeStatus(noSignal, wStatusData);
                }

                // теперь лампочка актуальная, обработаем сигнал
                if (wStatusData.Status == signal.Status && wStatusData.HasSignal == signal.HasSignal)
                {
                    // продлим
                    ProlongStatus(signal, wStatusData);
                }
                else
                {
                    // изменим
                    ChangeStatus(signal, wStatusData);
                }

                // запомним к какому статусу привело событие
                // сервис юнит-тестов сам обновит LastStatusId (в целях оптимизации) 
                // if (wStatusData.EventCategory != EventCategory.UnitTestStatus)
                {
                    UpdateLastStatusId(signal.EventId, wStatusData);
                }
                wStatusData.BeginSave();
            }

            return rBulb;
        }

        private void ChangeStatus(BulbSignal signal, BulbCacheWriteObject data)
        {
            // проверим пробел
            var startDate = GetStartDate(data, signal);
            if (startDate > data.ActualDate)
            {
                throw new Exception("startDate > data.ActualDate");
            }

            // обновим статус
            data.PreviousStatus = data.Status; // запомним предыдущий
            if (signal.EventId != Guid.Empty)
            {
                data.FirstEventId = signal.EventId;
                data.LastEventId = signal.EventId;
            }
            data.Count = 1;
            data.Message = signal.Message;
            data.Status = signal.Status;
            data.LastChildBulbId = signal.ChildBulbId;

            if (data.IsLeaf)
            {
                data.ActualDate = signal.ActualDate;
            }
            else
            {
                // время актуальности колбасок, которые НЕ являются листьями дерева, бесконечо
                data.ActualDate = DateTimeHelper.InfiniteActualDate;
            }

            data.StartDate = startDate;
            data.HasSignal = signal.HasSignal;
            data.EndDate = startDate; // меняется только при создании нового статуса

            var ownerId = data.GetOwnerId();
            AddStatusEvent(data, ownerId);
        }

        public IBulbCacheReadObject GetRaw(Guid statusId)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = statusId
            };
            return AllCaches.StatusDatas.Find(request);
        }

        public IBulbCacheReadObject SetSignal(Guid bulbId, BulbSignal signal)
        {
            var statusData = ProlongOrChangeStatus(signal, bulbId);
            UpdateParentByChild(statusData);
            return statusData;
        }

        public IBulbCacheReadObject SetUnknownStatus(Guid statusId, DateTime processDate)
        {
            var signal = BulbSignal.CreateUnknown(processDate);
            var data = ProlongOrChangeStatus(signal, statusId);
            UpdateParentByChild(data);
            return data;
        }

        private void UpdateParentByChild(IBulbCacheReadObject childBulb)
        {
            if (childBulb == null)
            {
                return;
            }

            var parentId = childBulb.GetParentId();
            if (parentId == null)
            {
                return;
            }

            var parentRequest = new AccountCacheRequest()
            {
                ObjectId = parentId.Value
            };
            IBulbCacheReadObject rParent = null;

            using (var parent = AllCaches.StatusDatas.Write(parentRequest))
            {
                // менять выключенного родителя нельзя, его надо сначала включить
                if (parent.Status == MonitoringStatus.Disabled)
                {
                    return;
                }

                var processDate = DateTime.Now;

                BulbSignal signal = null;

                if (childBulb.Status > parent.Status)
                {
                    // 1. если дочерний опаснее
                    signal = BulbSignal.CreateFromChild(processDate, childBulb);
                    rParent = ProlongOrChangeStatus(signal, parent.Id);
                }
                else
                {
                    // 2. если дочерний менее опасный и он был последней причиной
                    if (childBulb.Status < parent.Status && parent.LastChildBulbId == childBulb.Id)
                    {
                        // нужно расчитать занова
                        CalculateByChilds(parent);
                    }
                }
            }
            if (rParent != null)
            {
                UpdateParentByChild(rParent);
            }
        }

        private void CalculateByChilds(IBulbCacheReadObject bulb)
        {
            if (bulb.ComponentId == null)
                throw new ArgumentNullException("bulb.ComponentId");

            var componentId = bulb.ComponentId.Value;

            // ComponentExternalStatus
            if (bulb.EventCategory == EventCategory.ComponentExternalStatus)
            {
                var component = _storage.Components.GetOneById(componentId);
                var childsId = new[]
                {
                    component.ChildComponentsStatusId,
                    component.InternalStatusId
                };
                CalculateByChilds(bulb.Id, childsId);
            }
            // ComponentChildsStatus
            else if (bulb.EventCategory == EventCategory.ComponentChildsStatus)
            {
                var childsId = _storage.Components.GetChilds(componentId).Select(x => x.ExternalStatusId).ToArray();
                CalculateByChilds(bulb.Id, childsId);
            }
            // ComponentInternalStatus
            else if (bulb.EventCategory == EventCategory.ComponentInternalStatus)
            {
                var component = _storage.Components.GetOneById(componentId);
                var childsId = new[]
                {
                    component.UnitTestsStatusId,
                    component.MetricsStatusId,
                    component.EventsStatusId
                };
                CalculateByChilds(bulb.Id, childsId);
            }
            // ComponentUnitTestsStatus
            else if (bulb.EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                var childsId = _storage.UnitTests.GetByComponentId(componentId).Select(x => x.StatusDataId).ToArray();
                CalculateByChilds(bulb.Id, childsId);
            }
            // ComponentMetricsStatus
            else if (bulb.EventCategory == EventCategory.ComponentMetricsStatus)
            {
                var childsId = _storage.Metrics.GetByComponentId(componentId).Select(x => x.StatusDataId).ToArray();
                CalculateByChilds(bulb.Id, childsId);
            }
            // ComponentEventsStatus
            else if (bulb.EventCategory == EventCategory.ComponentEventsStatus)
            {
                // данный перерасчет делает ComponentService
            }
        }

        public void CalculateByChilds(Guid statusId, Guid[] childs)
        {
            var processDate = DateTime.Now;
            var request = new AccountCacheRequest()
            {
                ObjectId = statusId
            };
            IBulbCacheReadObject rBulb = null;
            using (var statusData = AllCaches.StatusDatas.Write(request))
            {
                var childsList = new List<IBulbCacheReadObject>();
                foreach (var childId in childs)
                {
                    var childRequest = new AccountCacheRequest()
                    {
                        ObjectId = childId
                    };
                    var child = AllCaches.StatusDatas.Find(childRequest);

                    // Disabled и Unknown не влияют на родителей
                    if (child != null && child.Status != MonitoringStatus.Disabled && child.Status != MonitoringStatus.Unknown)
                    {
                        childsList.Add(child);
                    }
                }
                var mostDandger = childsList.OrderByDescending(x => x.Status).FirstOrDefault();
                if (mostDandger == null)
                {
                    var signal = BulbSignal.CreateUnknown(processDate);
                    rBulb = ProlongOrChangeStatus(signal, statusId);
                }
                else
                {
                    var signal = BulbSignal.CreateFromChild(processDate, mostDandger);
                    rBulb = ProlongOrChangeStatus(signal, statusId);
                }
            }
            UpdateParentByChild(rBulb);
        }

        // TODO move to bulb repository?
        public BulbForRead GetParent(BulbForRead bulb)
        {
            if (bulb.EventCategory == EventCategory.UnitTestStatus)
            {
                if (bulb.UnitTestId == null)
                {
                    return null;
                }
                var unittest = _storage.UnitTests.GetOneById(bulb.UnitTestId.Value);
                var component = _storage.Components.GetOneById(unittest.ComponentId);
                return _storage.Bulbs.GetOneById(component.UnitTestsStatusId);
            }
            if (bulb.EventCategory == EventCategory.MetricStatus)
            {
                if (bulb.MetricId == null)
                {
                    return null;
                }
                var metric = _storage.Metrics.GetOneById(bulb.MetricId.Value);
                var component = _storage.Components.GetOneById(metric.ComponentId);
                return _storage.Bulbs.GetOneById(component.MetricsStatusId);
            }
            if (bulb.ComponentId == null)
            {
                return null;
            }
            if (bulb.EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return _storage.Bulbs.GetOneById(component.InternalStatusId);
            }
            if (bulb.EventCategory == EventCategory.ComponentMetricsStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return _storage.Bulbs.GetOneById(component.InternalStatusId);
            }
            if (bulb.EventCategory == EventCategory.ComponentEventsStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return _storage.Bulbs.GetOneById(component.InternalStatusId);
            }
            if (bulb.EventCategory == EventCategory.ComponentChildsStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return _storage.Bulbs.GetOneById(component.ExternalStatusId);
            }
            if (bulb.EventCategory == EventCategory.ComponentInternalStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return _storage.Bulbs.GetOneById(component.ExternalStatusId);
            }
            if (bulb.EventCategory == EventCategory.ComponentExternalStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                if (component.IsRoot())
                {
                    return null;
                }
                var parent = _storage.Components.GetOneById(component.ParentId.Value);
                return _storage.Bulbs.GetOneById(parent.ChildComponentsStatusId);
            }
            throw new Exception("Неизвестное значение EventCategory: " + bulb.EventCategory);
        }

        // TODO Move to bulb repository?
        public BulbForRead[] GetChilds(BulbForRead bulb)
        {
            if (bulb.EventCategory == EventCategory.UnitTestStatus)
            {
                return new BulbForRead[0]; // нет детей
            }
            if (bulb.EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                var unittests = _storage.UnitTests.GetByComponentId(bulb.ComponentId.Value);
                var childs = unittests.Select(x => _storage.Bulbs.GetOneById(x.StatusDataId)).ToArray();
                return childs;
            }
            if (bulb.EventCategory == EventCategory.MetricStatus)
            {
                return new BulbForRead[0]; // нет детей
            }
            if (bulb.EventCategory == EventCategory.ComponentMetricsStatus)
            {
                var metrics = _storage.Metrics.GetByComponentId(bulb.ComponentId.Value);
                var childs = metrics.Select(x => _storage.Bulbs.GetOneById(x.StatusDataId)).ToArray();
                return childs;
            }
            if (bulb.EventCategory == EventCategory.ComponentEventsStatus)
            {
                return new BulbForRead[0]; // нет детей
            }
            if (bulb.EventCategory == EventCategory.ComponentChildsStatus)
            {
                var childs = _storage.Components.GetChilds(bulb.ComponentId.Value)
                    .Select(x => _storage.Bulbs.GetOneById(x.ExternalStatusId)).ToArray();
                return childs;
            }
            if (bulb.EventCategory == EventCategory.ComponentInternalStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return new BulbForRead[]
                {
                    _storage.Bulbs.GetOneById(component.UnitTestsStatusId),
                    _storage.Bulbs.GetOneById(component.MetricsStatusId),
                    _storage.Bulbs.GetOneById(component.EventsStatusId)
                };
            }
            if (bulb.EventCategory == EventCategory.ComponentExternalStatus)
            {
                var component = _storage.Components.GetOneById(bulb.ComponentId.Value);
                return new BulbForRead[]
                {
                    _storage.Bulbs.GetOneById(component.InternalStatusId),
                    _storage.Bulbs.GetOneById(component.ChildComponentsStatusId)
                };
            }
            throw new Exception("Неизвестное значение EventCategory: " + bulb.EventCategory);
        }

        public Guid GetComponentId(BulbForRead bulb)
        {
            // если это колбаска проверки
            if (bulb.EventCategory == EventCategory.UnitTestStatus)
            {
                var unittest = _storage.UnitTests.GetOneById(bulb.UnitTestId.Value);
                return unittest.ComponentId;
            }

            // если это колбаска метрики
            if (bulb.EventCategory == EventCategory.MetricStatus)
            {
                var metric = _storage.Metrics.GetOneById(bulb.MetricId.Value);
                return metric.ComponentId;
            }

            // все другие колбаски - это колбаски компонента
            return bulb.ComponentId.Value;
        }

        public Guid CreateBulb(DateTime createDate,
            EventCategory eventCategory,
            Guid ownerId,
            string statusMessage)
        {
            var result = new BulbForAdd()
            {
                Id = Ulid.NewUlid(),
                StartDate = createDate,
                EndDate = createDate,
                ActualDate = DateTimeHelper.InfiniteActualDate,
                EventCategory = eventCategory,
                Count = 1,
                HasSignal = false,
                Status = MonitoringStatus.Unknown,
                Message = statusMessage,
                UpTimeStartDate = createDate,
                CreateDate = createDate,
                StatusEventId = Guid.Empty
            };
            _storage.Bulbs.Add(result);

            var request = new AccountCacheRequest()
            {
                ObjectId = result.Id
            };
            using (var wStatus = AllCaches.StatusDatas.Write(request))
            {
                AddStatusEvent(wStatus, ownerId);
                wStatus.BeginSave();
            }

            // выгрузим, т.к. сейчас неполные данные, еще не установлен ComponentId
            AllCaches.StatusDatas.Unload(request);
            return result.Id;
        }

        public IBulbCacheReadObject GetActual(
            Guid statusId,
            DateTime processDate,
            EventImportance noSignalImportance)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = statusId
            };
            var data = AllCaches.StatusDatas.Find(request);

            // статус актуальный
            if (data.Actual(processDate))
            {
                return data;
            }

            // статус протух
            IBulbCacheReadObject rStatus = null;
            using (var rStatus2 = AllCaches.StatusDatas.Write(request))
            {
                rStatus = rStatus2;
                if (rStatus.Actual(processDate))
                {
                    return rStatus;
                }

                var signal = BulbSignal.CreateNoSignal(data.ActualDate, noSignalImportance);

                ProlongOrChangeStatus(signal, statusId);
            }

            UpdateParentByChild(rStatus);
            return rStatus;
        }
    }
}
