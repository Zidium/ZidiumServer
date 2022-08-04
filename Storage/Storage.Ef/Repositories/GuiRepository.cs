using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    internal class GuiRepository : IGuiRepository
    {
        public GuiRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public GetGuiChecksResultsInfo[] GetChecksResults()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.Component)
                    .Include(t => t.Component.ComponentType)
                    .Include(t => t.Type)
                    .Include(t => t.Bulb)
                    .Select(t => new GetGuiChecksResultsInfo()
                    {
                        Id = t.Id,
                        Component = new GetGuiChecksResultsInfo.ComponentInfo()
                        {
                            Id = t.ComponentId,
                            ComponentType = new GetGuiChecksResultsInfo.ComponentTypeInfo()
                            {
                                Id = t.Component.ComponentTypeId,
                                DisplayName = t.Component.ComponentType.DisplayName
                            }
                        },
                        Type = new GetGuiChecksResultsInfo.TypeInfo()
                        {
                            Id = t.TypeId,
                            DisplayName = t.Type.DisplayName,
                            IsSystem = t.Type.IsSystem
                        },
                        Bulb = new GetGuiChecksResultsInfo.BulbInfo()
                        {
                            Status = t.Bulb.Status
                        }
                    })
                    .ToArray();
            }
        }

        public GetGuiComponentHistoryInfo[] GetComponentHistory()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.EventsStatus)
                    .Select(t => new GetGuiComponentHistoryInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.DisplayName,
                        SystemName = t.SystemName,
                        ParentId = t.ParentId,
                        ComponentTypeId = t.ComponentTypeId,
                        HasEvents = t.EventsStatus.FirstEventId.HasValue,
                        UnitTests = t.UnitTests.Where(a => !a.IsDeleted).Select(x => new GetGuiComponentHistoryInfo.UnitTestInfo()
                        {
                            Id = x.Id,
                            DisplayName = x.DisplayName
                        }).ToList(),
                        Metrics = t.Metrics.Where(a => !a.IsDeleted && !a.MetricType.IsDeleted).Select(x => new GetGuiComponentHistoryInfo.MetricInfo()
                        {
                            Id = x.Id,
                            DisplayName = x.MetricType.DisplayName
                        }).ToList()
                    })
                    .ToArray();
            }
        }

        public GetGuiComponentListInfo[] GetComponentList(
            Guid? componentTypeId,
            Guid[] excludeComponentTypeIds,
            Guid? parentComponentId,
            MonitoringStatus[] statuses,
            string search,
            int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.ExternalStatus)
                    .Include(t => t.ComponentType)
                    .AsQueryable();

                if (componentTypeId == null && parentComponentId == null)
                {
                    query = query.Where(t => !excludeComponentTypeIds.Contains(t.ComponentTypeId));
                }

                if (parentComponentId.HasValue)
                {
                    query = query.Where(x => x.ParentId == parentComponentId);
                }

                if (componentTypeId.HasValue)
                    query = query.Where(t => t.ComponentTypeId == componentTypeId);

                if (statuses != null)
                    query = query.Where(t => statuses.Contains(t.ExternalStatus.Status));

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t => t.Id.ToString().ToLower().Contains(search) ||
                                             t.SystemName.ToLower().Contains(search) ||
                                             t.DisplayName.ToLower().Contains(search));
                }

                query = query.OrderBy(t => t.DisplayName).Take(maxCount);

                return query.Select(t => new GetGuiComponentListInfo()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Version = t.Version,
                    ExternalStatus = t.ExternalStatus.Status,
                    ComponentType = new GetGuiComponentListInfo.ComponentTypeInfo()
                    {
                        Id = t.ComponentTypeId,
                        DisplayName = t.ComponentType.DisplayName
                    }
                }).ToArray();
            }
        }

        public GetGuiComponentShowInfo GetComponentShow(Guid id, DateTime now)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var component = contextWrapper.Context.Components.First(t => t.Id == id);
                var componentEntry = contextWrapper.Context.Entry(component);

                var result = new GetGuiComponentShowInfo()
                {
                    UnitTests = componentEntry.Collection(t => t.UnitTests).Query()
                    .Include(t => t.Bulb)
                    .Include(t => t.Type)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.UnitTestInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.DisplayName,
                        Enable = t.Enable,
                        PeriodSeconds = t.PeriodSeconds,
                        Bulb = BulbRepository.DbToEntity(t.Bulb),
                        Type = new GetGuiComponentShowInfo.UnitTestTypeInfo()
                        {
                            Id = t.TypeId,
                            DisplayName = t.Type.DisplayName,
                            IsSystem = t.Type.IsSystem
                        }
                    }).ToArray(),
                    Metrics = componentEntry.Collection(t => t.Metrics).Query()
                    .Include(t => t.Bulb)
                    .Include(t => t.MetricType)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.MetricInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.MetricType.DisplayName,
                        Enable = t.Enable,
                        Value = t.Value,
                        Bulb = BulbRepository.DbToEntity(t.Bulb),
                        MetricType = new GetGuiComponentShowInfo.MetricTypeInfo()
                        {
                            Id = t.MetricTypeId,
                            DisplayName = t.MetricType.DisplayName
                        }
                    }).ToArray(),
                    Childs = componentEntry.Collection(t => t.Childs).Query()
                    .Include(t => t.ExternalStatus)
                    .Include(t => t.ComponentType)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.ChildInfo()
                    {
                        Id = t.Id,
                        ComponentType = new GetGuiComponentShowInfo.ComponentTypeInfo()
                        {
                            Id = t.ComponentTypeId,
                            DisplayName = t.ComponentType.DisplayName
                        },
                        DisplayName = t.DisplayName,
                        Enable = t.Enable,
                        ExternalStatus = BulbRepository.DbToEntity(t.ExternalStatus)
                    }).ToArray(),
                    UnitTestsMiniInfo = componentEntry.Collection(t => t.UnitTests).Query()
                    .Include(t => t.Bulb)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.UnitTestMiniInfo()
                    {
                        Id = t.Id,
                        Status = t.Bulb.Status
                    }).ToArray(),
                    ChildsMiniInfo = componentEntry.Collection(t => t.Childs).Query()
                    .Include(t => t.ExternalStatus)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.ChildMiniInfo()
                    {
                        Id = t.Id,
                        Status = t.ExternalStatus.Status
                    }).ToArray(),
                    MetricsMiniInfo = componentEntry.Collection(t => t.Metrics).Query()
                    .Include(t => t.Bulb)
                    .Where(t => !t.IsDeleted).Select(t => new GetGuiComponentShowInfo.MetricMiniInfo()
                    {
                        Id = t.Id,
                        Status = t.Bulb.Status
                    }).ToArray()
                };

                var categories = new[]
                {
                    EventCategory.ApplicationError,
                    EventCategory.ComponentEvent
                };

                result.ActualEventsMiniInfo = contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => t.OwnerId == id && t.ActualDate >= now && categories.Contains(t.Category))
                    .Select(t => new GetGuiComponentShowInfo.ActualEventMiniInfo()
                    {
                        Id = t.Id,
                        Importance = t.Importance
                    }).ToArray();

                return result;
            }
        }

        public GetGuiComponentMiniListInfo[] GetComponentMiniList()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.ExternalStatus)
                    .Select(t => new GetGuiComponentMiniListInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.DisplayName,
                        SystemName = t.SystemName,
                        Status = t.ExternalStatus.Status,
                        ComponentTypeId = t.ComponentTypeId
                    })
                    .ToArray();
            }
        }

        public GetGuiComponentStatesInfo[] GetComponentStates(
            Guid? componentTypeId,
            Guid? componentId,
            Guid? parentId,
            Guid[] excludeTypes,
            MonitoringStatus[] statuses,
            string searchString,
            DateTime now)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.ComponentType)
                    .AsQueryable();

                // фильтр по типу компонента
                if (componentTypeId.HasValue)
                {
                    query = query.Where(t => t.ComponentTypeId == componentTypeId.Value);
                }

                // фильтр по компоненту
                if (componentId.HasValue)
                {
                    query = query.Where(t => t.Id == componentId.Value);
                }
                else if (parentId.HasValue)
                {
                    query = query.Where(t => t.ParentId == parentId.Value);
                }
                else
                {
                    query = query.Where(x => !excludeTypes.Contains(x.ComponentTypeId));
                }

                // фильтр по цвету
                if (statuses != null && statuses.Length > 0)
                {
                    query = query.Where(t => statuses.Contains(t.ExternalStatus.Status));
                }

                // фильтр по строке
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    query = query.Where(t =>
                        t.SystemName.ToLower().Contains(searchString) ||
                        t.DisplayName.ToLower().Contains(searchString) ||
                        t.ComponentType.SystemName.ToLower().Contains(searchString) ||
                        t.ComponentType.DisplayName.ToLower().Contains(searchString));
                }

                var categories = new[]
                {
                    EventCategory.ApplicationError,
                    EventCategory.ComponentEvent
                };

                var result = query.Select(t => new GetGuiComponentStatesInfo()
                {
                    Id = t.Id,
                    SystemName = t.SystemName,
                    DisplayName = t.DisplayName,
                    CreatedDate = t.CreatedDate,
                    ComponentType = new GetGuiComponentStatesInfo.ComponentTypeInfo()
                    {
                        Id = t.ComponentTypeId,
                        DisplayName = t.ComponentType.DisplayName
                    },
                    UnitTests = t.UnitTests.Where(x => !x.IsDeleted).Select(x => new GetGuiComponentStatesInfo.UnitTestInfo()
                    {
                        Id = x.Id,
                        Status = x.Bulb.Status
                    }).ToList(),
                    Metrics = t.Metrics.Where(x => !x.IsDeleted).Select(x => new GetGuiComponentStatesInfo.MetricInfo()
                    {
                        Id = x.Id,
                        Status = x.Bulb.Status
                    }).ToList(),
                    Childs = t.Childs.Where(x => !x.IsDeleted).Select(x => new GetGuiComponentStatesInfo.ChildInfo()
                    {
                        Id = x.Id,
                        Status = x.ExternalStatus.Status
                    }).ToList(),
                    Events = contextWrapper.Context.Events
                        .Where(x => x.OwnerId == t.Id && x.ActualDate >= now && categories.Contains(x.Category))
                        .Select(x => new GetGuiComponentStatesInfo.EventInfo()
                        {
                            Id = x.Id,
                            Importance = x.Importance
                        }).ToList()
                }).ToArray();

                return result;
            }
        }

        public GetGuiTimelinesUnitTestsInfo[] GetTimelinesUnitTests(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var unitTestIds = contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => t.ComponentId == componentId)
                    .Select(t => t.Id).ToArray();

                var ids = contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => unitTestIds.Contains(t.OwnerId) &&
                                t.Category == EventCategory.UnitTestStatus &&
                                importances.Contains(t.Importance) &&
                                t.StartDate <= toDate &&
                                t.ActualDate >= fromDate)
                    .GroupBy(t => t.OwnerId)
                    .Select(t => t.Key)
                    .ToArray();

                return contextWrapper.Context.UnitTests.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .Select(t => new GetGuiTimelinesUnitTestsInfo()
                    {
                        UnitTestId = t.Id,
                        DisplayName = t.DisplayName
                    })
                    .ToArray();
            }
        }

        public GetGuiTimelinesMetricsInfo[] GetTimelinesMetrics(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var metricIds = contextWrapper.Context.Metrics.AsNoTracking()
                    .Where(t => t.ComponentId == componentId)
                    .Select(t => t.Id).ToArray();

                var ids = contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => metricIds.Contains(t.OwnerId) &&
                                t.Category == EventCategory.MetricStatus &&
                                importances.Contains(t.Importance) &&
                                t.StartDate <= toDate &&
                                t.ActualDate >= fromDate)
                    .GroupBy(t => t.OwnerId)
                    .Select(t => t.Key)
                    .ToArray();

                return contextWrapper.Context.Metrics.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .Include(t => t.MetricType)
                    .Select(t => new GetGuiTimelinesMetricsInfo()
                    {
                        MetricId = t.Id,
                        DisplayName = t.MetricType.DisplayName
                    })
                    .ToArray();
            }
        }

        public GetGuiTimelinesChildsInfo[] GetTimelinesChilds(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var childIds = contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => t.ParentId == componentId)
                    .Select(t => t.Id).ToArray();

                var ids = contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => childIds.Contains(t.OwnerId) &&
                                t.Category == EventCategory.ComponentExternalStatus &&
                                importances.Contains(t.Importance) &&
                                t.StartDate <= toDate &&
                                t.ActualDate >= fromDate)
                    .GroupBy(t => t.OwnerId)
                    .Select(t => t.Key)
                    .ToArray();

                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .Select(t => new GetGuiTimelinesChildsInfo()
                    {
                        ComponentId = t.Id,
                        DisplayName = t.DisplayName
                    })
                    .ToArray();
            }
        }

        public GetGuiTimelinesEventsInfo[] GetTimelinesEvents(
            Guid componentId,
            EventImportance[] importances,
            DateTime fromDate,
            DateTime toDate)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var categories = new[]
{
                    EventCategory.ApplicationError,
                    EventCategory.ComponentEvent
                };

                return contextWrapper.Context.Events.AsNoTracking()
                    .Where(t => t.OwnerId == componentId &&
                                categories.Contains(t.Category) &&
                                importances.Contains(t.Importance) &&
                                t.StartDate <= toDate &&
                                t.ActualDate >= fromDate)
                    .GroupBy(t => new { EventTypeId = t.EventType.Id, DisplayName = t.EventType.DisplayName, Code = t.EventType.Code })
                    .Select(t => t.Key)
                    .Select(t => new GetGuiTimelinesEventsInfo()
                    {
                        EventTypeId = t.EventTypeId,
                        DisplayName = t.DisplayName,
                        Code = t.Code,
                        LastMessage = contextWrapper.Context.Events
                          .Where(z => z.OwnerId == componentId &&
                                z.EventTypeId == t.EventTypeId &&
                                categories.Contains(z.Category) &&
                                importances.Contains(z.Importance) &&
                                z.StartDate <= toDate &&
                                z.ActualDate >= fromDate)
                          .OrderByDescending(z => z.StartDate)
                          .Select(z => z.Message).FirstOrDefault()
                    }).ToArray();
            }
        }

        public GetGuiComponentMiniTreeInfo[] GetComponentMiniTree()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.ExternalStatus)
                    .Select(t => new GetGuiComponentMiniTreeInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.DisplayName,
                        Status = t.ExternalStatus.Status,
                        SystemName = t.SystemName,
                        ComponentTypeId = t.ComponentTypeId,
                        ParentId = t.ParentId
                    }).ToArray();
            }
        }

        public GetGuiSimplifiedComponentListInfo[] GetSimplifiedComponentList()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Components.AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Select(t => new GetGuiSimplifiedComponentListInfo()
                    {
                        Id = t.Id,
                        DisplayName = t.DisplayName,
                        SystemName = t.SystemName,
                        ComponentTypeId = t.ComponentTypeId,
                        ParentId = t.ParentId
                    })
                    .ToArray();
            }
        }

        public GetGuiDefectsInfo[] GetDefects(string title)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Defects.AsNoTracking()
                    .Include(t => t.LastChange)
                    .Include(t => t.ResponsibleUser)
                    .Include(t => t.EventType).AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    title = title.ToLower().Trim();
                    query = query.Where(t => t.Title.ToLower().Contains(title) ||
                        t.EventType.Code != null && t.EventType.Code.Contains(title));
                }

                return query.Select(t => new GetGuiDefectsInfo()
                {
                    Id = t.Id,
                    EventTypeId = t.EventTypeId,
                    Number = t.Number,
                    EventType = new GetGuiDefectsInfo.EventTypeInfo()
                    {
                        OldVersion = t.EventType.OldVersion,
                        Code = t.EventType.Code,
                    },
                    ResponsibleUserId = t.ResponsibleUserId,
                    ResponsibleUser = new GetGuiDefectsInfo.ResponsibleUserInfo()
                    {
                        NameOrLogin = t.ResponsibleUser.DisplayName
                    },
                    Title = t.Title,
                    LastChange = new GetGuiDefectsInfo.LastChangeInfo()
                    {
                        Status = t.LastChange.Status,
                        Date = t.LastChange.Date,
                        Comment = t.LastChange.Comment
                    }
                }).ToArray();
            }
        }
    }
}
