using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.AccountsDb
{
    public class BulbService : IBulbService
    {
        protected DispatcherContext Context { get; set; }

        public BulbService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }
        
        protected IEventCacheReadObject CreateOrUpdateStatusEvent(Guid accountId, BulbCacheWriteObject data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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
            return AddStatusEvent(accountId, data, ownerId);
        }

        protected IEventCacheReadObject AddStatusEvent(Guid accountId, BulbCacheWriteObject data, Guid ownerId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var eventType = SystemEventType.GetStatusEventType(data.EventCategory);
            var now = DateTime.Now;

            // создаем новый статус
            var newStatus = new Event()
            {
                Id = Guid.NewGuid(),
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
            var eventRepository = accountDbContext.GetEventRepository();
            eventRepository.Add(newStatus);

            // обновим старый статус
            var oldStatus = eventRepository.GetByIdOrNull(data.StatusEventId);
            if (oldStatus != null)
            {
                // установим флажок, если это архивный статус
                if (oldStatus.Category == EventCategory.ComponentExternalStatus)
                {
                    var archivedStatus = new ArchivedStatus()
                    {
                        EventId = oldStatus.Id
                    };
                    var archivedStatusRepository = accountDbContext.GetArchivedStatusRepository();
                    archivedStatusRepository.Add(archivedStatus);
                }

                // старый статус завершается, когда начинается новый статус
                oldStatus.ActualDate = newStatus.StartDate;
                oldStatus.EndDate = newStatus.StartDate;
                oldStatus.LastUpdateDate = now;

                // убедимся, что очереди изменений нет изменений старого статуста,
                // иначе наши изменения через EF могут быть перезатёрты
                using (var oldStatusCache = AllCaches.Events.GetForWrite(oldStatus, accountId))
                {
                    oldStatusCache.WaitSaveChanges();
                    oldStatusCache.Unload();
                }
            }

            // обновим лампочку через EF, чтобы {лампочка, старый статус и новый статус} сохранились одновременно
            // изменения лампочки будут сохранены дважды, воторой раз в очереди изменений
            // сейчас не понятно как этого избежать, дело в том, что после данного метода код может изменить кэш лампочки
            // и чтобы не потерять эти изменения сохраняем дважды
            data.StatusEventId = newStatus.Id;
            var bulbEF = accountDbContext.GetStatusDataRepository().GetById(data.Id);
            bulbEF.Count = data.Count;
            bulbEF.ActualDate = data.ActualDate;
            bulbEF.EndDate = data.EndDate;
            bulbEF.FirstEventId = data.FirstEventId;
            bulbEF.HasSignal = data.HasSignal;
            bulbEF.LastEventId = data.LastEventId;
            bulbEF.Message = data.Message;
            bulbEF.PreviousStatus = data.PreviousStatus;
            bulbEF.StartDate = data.StartDate;
            bulbEF.Status = data.Status;
            bulbEF.StatusEventId = data.StatusEventId;
            bulbEF.LastChildBulbId = data.LastChildBulbId;

            // убедимся что в очереди изменений нет других изменений лампочки, 
            // иначе сохранённые через EF изменения могут быть потеряны
            data.WaitSaveChanges();

            // todo нужно сохранять изменения в одном контексте (транзакции),
            // чтобы лампочка не потеряла ссылку на последний статус
            accountDbContext.SaveChanges();

            var rEvent = AllCaches.Events.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = newStatus.Id
            });
            return rEvent;
        }

        protected DateTime GetStartDate(IBulbCacheReadObject bulb, BulbSignal signal)
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

        protected void UpdateLastStatusId(Guid accountId, Guid eventId, IBulbCacheReadObject bulb)
        {
            if (eventId == Guid.Empty)
            {
                return;
            }
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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

        protected void ProlongStatus(BulbSignal signal, BulbCacheWriteObject data)
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
                data.ActualDate = EventHelper.InfiniteActualDate;
            }

            CreateOrUpdateStatusEvent(signal.AccountId, data);
        }

        protected IBulbCacheReadObject ProlongOrChangeStatus(BulbSignal signal, Guid statusDataId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = signal.AccountId,
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
                    var noSignal = BulbSignal.CreateNoSignal(signal.AccountId, wStatusData.ActualDate, signal.NoSignalImportance);
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
                    UpdateLastStatusId(signal.AccountId, signal.EventId, wStatusData);
                }
                wStatusData.BeginSave();
            }

            return rBulb;
        }

        protected void ChangeStatus(BulbSignal signal, BulbCacheWriteObject data)
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
                data.ActualDate = EventHelper.InfiniteActualDate;
            }

            data.StartDate = startDate;
            data.HasSignal = signal.HasSignal;
            data.EndDate = startDate; // меняется только при создании нового статуса

            var ownerId = data.GetOwnerId();
            var statusEvent = AddStatusEvent(signal.AccountId, data, ownerId);
        }

        public IBulbCacheReadObject GetRaw(Guid accountId, Guid statusId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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

        //public IBulbCacheReadObject Disable(Guid accountId, Guid statusId)
        //{
        //    var processDate = DateTime.Now;
        //    var signal = new BulbSignal()
        //    {
        //        AccountId = accountId,
        //        ActualDate = EventHelper.InfiniteActualDate, // чтобы статус был актуален вечно
        //        StartDate = processDate,
        //        ProcessDate = processDate,
        //        Status = MonitoringStatus.Disabled,
        //        Message = "Объект выключен",
        //        NoSignalImportance = EventImportance.Unknown // при выключении пробел будет серым
        //    };
        //    var statusData = ProlongOrChangeStatus(signal, statusId);
            
        //    // родителя не обновляем, т.к. его возможно нужно пересчитывать, а для этого нужно знать всех детей, а мы их не знаем
        //    //UpdateParentByChild(statusData);
            
        //    return statusData;
        //}

        

        protected Guid? GetParentStatusId(Guid accountId, Bulb data)
        {
            // если это колбаска проверки
            if (data.EventCategory == EventCategory.UnitTestStatus)
            {
                var unitTestRepository = Context.GetAccountDbContext(accountId).GetUnitTestRepository();
                var unitTest = unitTestRepository.GetById(data.GetOwnerId());
                return unitTest.Component.UnitTestsStatusId;
            }

            // если это колбаска метрики
            if (data.EventCategory == EventCategory.MetricStatus)
            {
                var metricRepository = Context.GetAccountDbContext(accountId).GetMetricRepository();
                var metric = metricRepository.GetById(data.GetOwnerId());
                return metric.Component.MetricsStatusId;
            }

            var componentRepository = Context.GetAccountDbContext(accountId).GetComponentRepository();
            var component = componentRepository.GetById(data.GetOwnerId());

            // если это колбаска событий
            if (data.EventCategory == EventCategory.ComponentEventsStatus)
            {
                return component.InternalStatusId;
            }

            // если это колбаска проверок
            if (data.EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                return component.InternalStatusId;
            }

            // если это колбаска метрик
            if (data.EventCategory == EventCategory.ComponentMetricsStatus)
            {
                return component.InternalStatusId;
            }

            // если это колбаска дочерних компонентов
            if (data.EventCategory == EventCategory.ComponentChildsStatus)
            {
                return component.InternalStatusId;
            }

            // если это колбаска внутреннего статуса
            if (data.EventCategory == EventCategory.ComponentInternalStatus)
            {
                return component.ExternalStatusId;
            }

            // если это колбаска внешнего статуса
            if (data.EventCategory == EventCategory.ComponentExternalStatus)
            {
                if (component.Parent == null)
                {
                    return null;
                }
                return component.Parent.ChildComponentsStatusId;
            }

            throw new Exception("Неизвестный тип колбаски " + data.EventCategory);
        }

        public IBulbCacheReadObject SetUnknownStatus(Guid accountId, Guid statusId, DateTime processDate)
        {
            var signal = BulbSignal.CreateUnknown(accountId, processDate);
            var data = ProlongOrChangeStatus(signal, statusId);
            UpdateParentByChild(data);
            return data;
        }

        protected void UpdateParentByChild(IBulbCacheReadObject childBulb)
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
                AccountId = childBulb.AccountId,
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
                    if (childBulb.Status < parent.Status &&  parent.LastChildBulbId == childBulb.Id)
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

        protected void CalculateByChilds(IBulbCacheReadObject bulb)
        {
            var accountId = bulb.AccountId;
            var accountDbContext = Context.GetAccountDbContext(accountId);

            // ComponentExternalStatus
            if (bulb.EventCategory == EventCategory.ComponentExternalStatus)
            {
                var component = accountDbContext.GetComponentRepository().GetById(bulb.ComponentId.Value);
                var childsId = new List<Guid>()
                {
                    component.ChildComponentsStatusId,
                    component.InternalStatusId
                };
                CalculateByChilds(accountId, bulb.Id, childsId);
            }
            // ComponentChildsStatus
            else if (bulb.EventCategory == EventCategory.ComponentChildsStatus)
            {
                var component = accountDbContext.GetComponentRepository().GetById(bulb.ComponentId.Value);
                var childsId = component.Childs.Where(x => x.IsDeleted == false).Select(x => x.ExternalStatusId).ToList();
                CalculateByChilds(accountId, bulb.Id, childsId);
            }
            // ComponentInternalStatus
            else if (bulb.EventCategory == EventCategory.ComponentInternalStatus)
            {
                var component = accountDbContext.GetComponentRepository().GetById(bulb.ComponentId.Value);
                var childsId = new List<Guid>()
                {
                    component.UnitTestsStatusId,
                    component.MetricsStatusId,
                    component.EventsStatusId
                };
                CalculateByChilds(accountId, bulb.Id, childsId);
            }
            // ComponentUnitTestsStatus
            else if (bulb.EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                var component = accountDbContext.GetComponentRepository().GetById(bulb.ComponentId.Value);
                var childsId = component.UnitTests.Where(x => x.IsDeleted == false).Select(x => x.StatusDataId).ToList();
                CalculateByChilds(accountId, bulb.Id, childsId);
            }
            // ComponentMetricsStatus
            else if (bulb.EventCategory == EventCategory.ComponentMetricsStatus)
            {
                var component = accountDbContext.GetComponentRepository().GetById(bulb.ComponentId.Value);
                var childsId = component.Metrics.Where(x => x.IsDeleted == false).Select(x => x.StatusDataId).ToList();
                CalculateByChilds(accountId, bulb.Id, childsId);
            }
            // ComponentEventsStatus
            else if (bulb.EventCategory == EventCategory.ComponentEventsStatus)
            {
                // данный перерасчет делает ComponentService
            }
        }

        public void CalculateByChilds(Guid accountId, Guid statusId, List<Guid> childs)
        {
            var processDate = DateTime.Now;
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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
                        AccountId = accountId,
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
                    var signal = BulbSignal.CreateUnknown(accountId, processDate);
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

        public Bulb CreateBulb(
            Guid accountId, 
            DateTime createDate, 
            EventCategory eventCategory,
            Guid ownerId)
        {
            var result = new Bulb()
            {
                Id = Guid.NewGuid(),
                StartDate = createDate,
                EndDate = createDate,
                ActualDate = EventHelper.InfiniteActualDate,
                EventCategory = eventCategory,
                Count = 1,
                HasSignal = false,
                Status = MonitoringStatus.Unknown,
                Message = "Нет данных",
                UpTimeStartDate = createDate,
                CreateDate = createDate,
                StatusEventId = Guid.Empty
            };
            var accountDbContext = Context.GetAccountDbContext(accountId);
            accountDbContext.Bulbs.Add(result);
            accountDbContext.SaveChanges();

            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = result.Id
            };
            using (var wStatus = AllCaches.StatusDatas.Write(request))
            {
                AddStatusEvent(accountId, wStatus, ownerId);
                wStatus.BeginSave();
            }

            // выгрузим, т.к. сейчас неполные данные, еще не установлен ComponentId
            AllCaches.StatusDatas.Unload(request);
            return result;
        }

        public IBulbCacheReadObject GetActual(
            Guid accountId, 
            Guid statusId, 
            DateTime processDate,
            EventImportance noSignalImportance)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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

                var signal = BulbSignal.CreateNoSignal(accountId, data.ActualDate, noSignalImportance);
                //var signal = new BulbSignal()
                //{
                //    AccountId = accountId,
                //    IsSpace = true,
                //    ActualDate = EventHelper.InfiniteActualDate,
                //    ProcessDate = processDate,
                //    StartDate = data.ActualDate, // data.ActualDate всегда меньше processDate
                //    Status = noSignalStatus,
                //    Message = "Нет данных",
                //    NoSignalImportance = noSignalImportance
                //};
                ProlongOrChangeStatus(signal, statusId);
            }
            UpdateParentByChild(rStatus);
            return rStatus;
        }
    }
}
