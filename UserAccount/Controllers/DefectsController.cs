using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Common;
using Zidium.UserAccount.Models.Defects;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DefectsController : ContextController
    {
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

            var baseQuery = CurrentAccountDbContext.Defects.AsQueryable()
                .Include("LastChange").Include("ResponsibleUser").Include("EventType");

            if (!string.IsNullOrEmpty(model.Title))
                baseQuery = baseQuery.Where(t => t.Title.Contains(model.Title));

            // Получим базовую информацию по дефектам
            var defects = baseQuery.Select(t => new DefectsIndexItemModel()
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
            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            // Если заполнен фильтр по компоненту и это не Root, то получим Id по этого компонента и всех вложенных
            var useComponentFilter = model.ComponentId.HasValue && model.ComponentId.Value != GetCurrentAccount().RootId;
            Guid[] componentIds = null;
            if (useComponentFilter)
            {
                componentIds = GetDispatcherClient().GetComponentAndChildIds(CurrentUser.AccountId, model.ComponentId.Value).Data;
            }

            foreach (var defect in defects)
            {
                // Посчитаем, сколько раз случался дефект
                var query = eventRepository.QueryAllByAccount();
                query = query.Where(t => t.EventTypeId == defect.EventTypeId);

                // Если настроена версионность, то посчитаем только "новые" события
                if (defect.OldVersion != null)
                {
                    var oldVersion = VersionHelper.FromString(defect.OldVersion) ?? -1;
                    query = query.Where(t => (t.VersionLong ?? long.MaxValue) > oldVersion);
                }

                // Группируем по компоненту, чтобы посчитать количество для выбранного компонента и вложенных
                var resultQuery = query.GroupBy(t => t.OwnerId).Select(t => new { ComponentId = t.Key, Count = t.Sum(x => x.Count) }).ToArray();

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

            var service = CurrentAccountDbContext.GetDefectService();
            var createUser = GetUserById(CurrentUser.Id);
            var responsibleUser = GetUserById(model.ResponsibleUserId.Value);
            service.CreateDefect(CurrentUser.AccountId, model.Title, createUser, responsibleUser, model.Notes);
            CurrentAccountDbContext.SaveChanges();

            return GetSuccessJsonResponse();
        }

        public ActionResult Show(Guid id)
        {
            var repository = CurrentAccountDbContext.GetDefectRepository();
            var defect = repository.GetById(id);
            var model = new ShowDefectModel()
            {
                Defect = defect
            };
            if (defect.EventTypeId.HasValue)
            {
                model.EventType = GetEventTypeById(defect.EventTypeId.Value);
            }
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var repository = CurrentAccountDbContext.GetDefectRepository();
            var defect = repository.GetById(id);
            var model = new EditDefectModel()
            {
                Id = id,
                Title = defect.Title,
                Notes = defect.Notes,
                UserId = defect.ResponsibleUserId,
                DefectCode = defect.GetCode()
            };
            return View(model);
        }

        [HttpPost]
        [CanEditAllData]
        [ValidateInput(false)]
        public ActionResult Edit(EditDefectModel model)
        {
            var repository = CurrentAccountDbContext.GetDefectRepository();
            var defect = repository.GetById(model.Id);
            if (ModelState.IsValid)
            {
                defect.Title = model.Title;
                defect.Notes = model.Notes;
                defect.ResponsibleUserId = model.UserId;
                CurrentAccountDbContext.SaveChanges();
                return RedirectToAction("Show", new { id = model.Id });
            }
            model.DefectCode = defect.GetCode();
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
            var eventType = GetEventTypeById(eventTypeId.Value);
            var defectService = CurrentAccountDbContext.GetDefectService();
            var createUser = GetUserById(CurrentUser.Id);
            var responsibleUser = GetUserById(userId.Value);
            var defect = defectService.GetOrCreateDefectForEventType(CurrentUser.AccountId, eventType, createUser, responsibleUser, comment);
            CurrentAccountDbContext.SaveChanges();
            return GetSuccessJsonResponse(new
            {
                defectId = defect.Id
            });
        }

        [CanEditAllData]
        public ActionResult ChangeStatusDialog(Guid defectId)
        {
            var defect = GetDefectById(defectId);
            var status = DefectStatus.Open;
            if (defect.LastChange != null)
            {
                status = defect.LastChange.Status;
            }
            var model = new ChangeDefectStatusDialogModel()
            {
                DefectId = defectId,
                DefectCode = defect.GetCode(),
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
            var defect = GetDefectById(model.DefectId.Value);
            var defectService = CurrentAccountDbContext.GetDefectService();
            var user = GetUserById(CurrentUser.Id);
            defectService.ChangeStatus(CurrentUser.AccountId, defect, model.Status.Value, user, model.Comment);
            CurrentAccountDbContext.SaveChanges();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult CreateAndCloseDefectForEventType(Guid eventTypeId)
        {
            var eventType = GetEventTypeById(eventTypeId);
            var defectService = CurrentAccountDbContext.GetDefectService();
            var user = GetUserById(CurrentUser.Id);
            var defect = defectService.CreateAndCloseDefectForEventType(CurrentUser.AccountId, eventType, user);
            CurrentAccountDbContext.SaveChanges();
            return GetSuccessJsonResponse(new
            {
                defectId = defect.Id,
                eventTypeId = eventType.Id
            });
        }

        public ActionResult LastError(Guid eventTypeId)
        {
            var repository = CurrentAccountDbContext.GetEventRepository();
            var lastEvent = repository.GetLastEventByEndDate(eventTypeId);
            var model = new DefectLastErrorModel()
            {
                Event = lastEvent
            };
            if (lastEvent != null)
            {
                model.Component = GetComponentById(lastEvent.OwnerId);
            }
            return PartialView(model);
        }

        public PartialViewResult DefectControl(Guid eventTypeId)
        {
            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = eventTypeRepository.GetById(eventTypeId);
            return PartialView(eventType);
        }
    }
}