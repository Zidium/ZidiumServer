using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models.Defects;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DefectsController : BaseController
    {
        public DefectsController(ILogger<DefectsController> logger) : base(logger)
        {
        }

        public ActionResult Index(
            DefectsIndexModel.ShowModeEnum? showMode,
            Guid? userId,
            Guid? componentId,
            string title)
        {
            var model = new DefectsIndexModel()
            {
                ShowMode = showMode ?? DefectsIndexModel.ShowModeEnum.InWork,
                UserId = userId,
                ComponentId = componentId,
                Title = title
            };

            // Подготовим базовый запрос

            var data = GetStorage().Gui.GetDefects(model.Title);

            // Получим базовую информацию по дефектам
            var defects = data.Select(t => new DefectsIndexItemModel()
            {
                Id = t.Id,
                EventTypeId = t.EventTypeId,
                LastChangeDate = t.LastChange.Date,
                Status = t.LastChange.Status,
                ResponsibleUserId = t.ResponsibleUserId,
                Code = t.Number.ToString(),
                Comment = t.LastChange.Comment,
                Title = t.Title,
                ResponsibleUser = t.ResponsibleUser,
                OldVersion = t.EventType.OldVersion
            }).ToArray();

            // Отфильтруем дефекты по компоненту
            var filteredDefects = new List<DefectsIndexItemModel>();
            var eventRepository = GetStorage().Events;

            // Если заполнен фильтр по компоненту и это не Root, то получим Id по этого компонента и всех вложенных
            var root = GetDispatcherClient().GetRoot().Data;
            var useComponentFilter = model.ComponentId.HasValue && model.ComponentId.Value != root.Id;
            Guid[] componentIds = null;
            if (useComponentFilter)
            {
                componentIds = GetDispatcherClient().GetComponentAndChildIds(model.ComponentId.Value).Data;
            }

            foreach (var defect in defects)
            {
                if (defect.EventTypeId.HasValue)
                {
                    // Посчитаем, сколько раз случался дефект
                    var events = eventRepository.GetByEventTypeId(defect.EventTypeId.Value);

                    // Если настроена версионность, то посчитаем только "новые" события
                    if (defect.OldVersion != null)
                    {
                        var oldVersion = VersionHelper.FromString(defect.OldVersion) ?? -1;
                        events = events.Where(t => (t.VersionLong ?? long.MaxValue) > oldVersion).ToArray();
                    }

                    // Группируем по компоненту, чтобы посчитать количество для выбранного компонента и вложенных
                    var resultQuery = events.GroupBy(t => t.OwnerId)
                        .Select(t => new { ComponentId = t.Key, Count = t.Sum(x => x.Count) }).ToArray();

                    if (useComponentFilter)
                    {
                        defect.Count = resultQuery.Where(t => componentIds.Contains(t.ComponentId)).Sum(t => t.Count);

                        // Если заполнен фильтр по компоненту и таких событий нет, то этот дефект не показываем
                        if (defect.Count == 0)
                            continue;
                    }
                    else
                    {
                        defect.Count = resultQuery.Sum(t => t.Count);
                    }
                }

                filteredDefects.Add(defect);
            }

            // Подготовим условия для каждого фильтра

            Expression<Func<DefectsIndexItemModel, bool>> inWorkFilter = x => DefectsIndexModel.InWorkStatuses.Contains(x.Status);
            Expression<Func<DefectsIndexItemModel, bool>> testingFilter = x => x.Status == DefectStatus.Testing;
            Expression<Func<DefectsIndexItemModel, bool>> closedFilter = x => x.Status == DefectStatus.Closed;

            Expression<Func<DefectsIndexItemModel, bool>> modeFilter = null;
            if (model.ShowMode == DefectsIndexModel.ShowModeEnum.InWork)
                modeFilter = inWorkFilter;
            else if (model.ShowMode == DefectsIndexModel.ShowModeEnum.Testing)
                modeFilter = testingFilter;
            else if (model.ShowMode == DefectsIndexModel.ShowModeEnum.Closed)
                modeFilter = closedFilter;

            Expression<Func<DefectsIndexItemModel, bool>> userFilter = null;
            if (model.UserId.HasValue)
                userFilter = x => x.ResponsibleUserId == model.UserId;

            // Посчитаем количество для каждого варианта фильтра по статусу

            var modeFilterCountQuery = filteredDefects.AsQueryable();

            if (userFilter != null)
                modeFilterCountQuery = modeFilterCountQuery.Where(userFilter);

            model.InWorkCount = modeFilterCountQuery.Count(inWorkFilter);
            model.TestingCount = modeFilterCountQuery.Count(testingFilter);
            model.ClosedCount = modeFilterCountQuery.Count(closedFilter);

            // Отберём данные по фильтрам

            var defectsQuery = filteredDefects.AsQueryable();

            if (userFilter != null)
                defectsQuery = defectsQuery.Where(userFilter);

            if (modeFilter != null)
                defectsQuery = defectsQuery.Where(modeFilter);

            model.Items = defectsQuery.ToArray();

            model.Items = model.Items.OrderByDescending(t => t.Count).ToArray();
            model.TotalCount = model.InWorkCount + model.TestingCount + model.ClosedCount;

            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add()
        {
            var model = new AddDefectModel();
            return PartialView(model);
        }

        [HttpPost]
        [CanEditAllData]
        public ActionResult Add(AddDefectModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var service = new DefectService(GetStorage());
            var createUser = GetStorage().Users.GetOneById(CurrentUser.Id);
            var responsibleUser = GetStorage().Users.GetOneById(model.ResponsibleUserId.Value);
            service.CreateDefect(model.Title, createUser, responsibleUser, model.Notes);

            return GetSuccessJsonResponse();
        }

        public ActionResult Show(Guid id)
        {
            var defect = GetStorage().Defects.GetOneById(id);
            var changes = GetStorage().DefectChanges.GetByDefectId(defect.Id).OrderByDescending(x => x.Date).ToArray();
            var lastChange = changes.First();
            var lastChangeUser = lastChange.UserId != null ? GetStorage().Users.GetOneById(lastChange.UserId.Value) : null;
            var responsibleUser = defect.ResponsibleUserId != null ? GetStorage().Users.GetOneById(defect.ResponsibleUserId.Value) : null;
            var changeUsers = GetStorage().Users.GetMany(changes.Where(t => t.UserId != null).Select(t => t.UserId.Value).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);

            var model = new ShowDefectModel()
            {
                Defect = defect,
                Changes = changes.Select(t => new ShowDefectModel.DefectChangeInfo()
                {
                    Status = t.Status,
                    Comment = t.Comment,
                    Date = t.Date,
                    UserLogin = t.UserId != null ? changeUsers[t.UserId.Value].Login : null
                }).ToArray(),
                LastChangeUser = lastChangeUser?.FioOrLogin(),
                ResponsibleUser = responsibleUser?.FioOrLogin()
            };
            if (defect.EventTypeId.HasValue)
            {
                model.EventType = GetStorage().EventTypes.GetOneById(defect.EventTypeId.Value);
            }
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var defect = GetStorage().Defects.GetOneById(id);
            var model = new EditDefectModel()
            {
                Id = id,
                Title = defect.Title,
                Notes = defect.Notes,
                UserId = defect.ResponsibleUserId,
                DefectCode = defect.Number.ToString()
            };
            return View(model);
        }

        [HttpPost]
        [CanEditAllData]
        public ActionResult Edit(EditDefectModel model)
        {
            var defect = GetStorage().Defects.GetOneById(model.Id);
            if (ModelState.IsValid)
            {
                var defectForUpdate = defect.GetForUpdate();
                defectForUpdate.Title.Set(model.Title);
                defectForUpdate.Notes.Set(model.Notes);
                defectForUpdate.ResponsibleUserId.Set(model.UserId);
                GetStorage().Defects.Update(defectForUpdate);

                return RedirectToAction("Show", new { id = model.Id });
            }

            model.DefectCode = defect.Number.ToString();
            return View(model);
        }

        [CanEditAllData]
        public ActionResult CreateDefectDialog(Guid eventTypeId)
        {
            var model = new CreateDefectDialogModel()
            {
                EventTypeId = eventTypeId,
                UserId = CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// ajax-запрос на установку ответственного за дефект ошибки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CanEditAllData]
        public ActionResult CreateDefectDialog(Guid? eventTypeId, Guid? userId, string comment)
        {
            if (eventTypeId == null)
            {
                throw new Exception("eventTypeId == null");
            }
            if (userId == null)
            {
                throw new Exception("userId == null");
            }

            var storage = GetStorage();
            var eventType = storage.EventTypes.GetOneById(eventTypeId.Value);
            var defectService = new DefectService(storage);
            var createUser = storage.Users.GetOneById(CurrentUser.Id);
            var responsibleUser = storage.Users.GetOneById(userId.Value);
            var defect = defectService.GetOrCreateDefectForEventType(eventType, createUser, responsibleUser, comment);

            return GetSuccessJsonResponse(new
            {
                defectId = defect.Id
            });
        }

        [CanEditAllData]
        public ActionResult ChangeStatusDialog(Guid defectId)
        {
            var defect = GetStorage().Defects.GetOneById(defectId);
            var status = DefectStatus.Open;
            if (defect.LastChangeId != null)
            {
                var lastChange = GetStorage().DefectChanges.GetOneById(defect.LastChangeId.Value);
                status = lastChange.Status;
            }
            var model = new ChangeDefectStatusDialogModel()
            {
                DefectId = defectId,
                DefectCode = defect.Number.ToString(),
                Status = status
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult ChangeStatusDialogPost(ChangeDefectStatusDialogModel model)
        {
            if (model.DefectId == null)
            {
                throw new Exception("model.DefectId == null");
            }
            if (model.Status == null)
            {
                throw new Exception("model.Status == null");
            }

            var storage = GetStorage();
            var defect = storage.Defects.GetOneById(model.DefectId.Value);
            var defectService = new DefectService(storage);
            var user = storage.Users.GetOneById(CurrentUser.Id);
            defectService.ChangeStatus(defect, model.Status.Value, user, model.Comment);

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult CreateAndCloseDefectForEventType(Guid eventTypeId)
        {
            var storage = GetStorage();
            var eventType = storage.EventTypes.GetOneById(eventTypeId);
            var defectService = new DefectService(storage);
            var user = storage.Users.GetOneById(CurrentUser.Id);
            var defectId = defectService.CreateAndCloseDefectForEventType(eventType, user);

            return GetSuccessJsonResponse(new
            {
                defectId = defectId,
                eventTypeId = eventType.Id
            });
        }

        public ActionResult LastError(Guid eventTypeId)
        {
            var storage = GetStorage();
            var lastEvent = storage.Events.GetLastEventByEndDate(eventTypeId);
            var model = new DefectLastErrorModel()
            {
                Event = lastEvent
            };
            if (lastEvent != null)
            {
                var componentService = new ComponentService(storage);
                var component = storage.Components.GetOneById(lastEvent.OwnerId);
                model.ComponentId = component.Id;
                model.ComponentFullName = componentService.GetFullDisplayName(component);

            }
            return PartialView(model);
        }

        public PartialViewResult DefectControl(Guid eventTypeId)
        {
            var eventType = GetStorage().EventTypes.GetOneById(eventTypeId);
            var defect = eventType.DefectId != null ? GetStorage().Defects.GetOneById(eventType.DefectId.Value) : null;
            var lastChange = defect != null ? GetStorage().DefectChanges.GetLastByDefectId(defect.Id) : null;

            var model = new DefectControlModel()
            {
                EventType = eventType,
                Defect = defect,
                LastChange = lastChange
            };
            return PartialView(model);
        }
    }
}