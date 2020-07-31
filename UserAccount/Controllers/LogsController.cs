using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class LogsController : BaseController
    {
        /// <summary>
        /// Главная страница лога
        /// </summary>
        public ActionResult Index(LogFiltersModel filters)
        {
            filters = filters ?? new LogFiltersModel();

            var model = new LogIndexModel()
            {
                ComponentId = filters.ComponentId,
                LogLevel = filters.LogLevel,
                Context = filters.Context,
                Date = filters.Date,
                Text = filters.Text,
                Id = filters.Id
            };

            if (model.ComponentId.HasValue)
            {
                var component = GetStorage().Components.GetOneById(model.ComponentId.Value);
                model.Component = new LogIndexModel.ComponentInfo()
                {
                    Id = component.Id,
                    DisplayName = component.DisplayName,
                    LogConfig = GetStorage().LogConfigs.GetOneByComponentId(component.Id)
                };
            }

            return View(model);
        }

        /// <summary>
        /// Подгружаемая часть лога, не для режима следующие N / предыдущие N
        /// </summary>
        public PartialViewResult Partial(LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            // Если явно указана запись, отобразим как при поиске, с центрированием
            if (filters.Id.HasValue)
                return Center(filters);

            var levels = GetFilterLevels(filters.LogLevel);

            LogForRead[] records;

            if (filters.Date.HasValue)
                records = GetStorage().Logs.GetFirstRecords(filters.ComponentId.Value, filters.Date, levels, filters.Context, OutputRecordCount);
            else
                records = GetStorage().Logs.GetLastRecords(filters.ComponentId.Value, null, levels, filters.Context, OutputRecordCount);

            var model = new LogPartialModel()
            {
                Filters = filters,
                Items = RecordsToModel(records),
                OutputRecordCount = OutputRecordCount
            };

            // Если показываем записи с конца, то прокрутим до последней записи
            if (!filters.Date.HasValue && model.Items.Length > 0)
                model.ScrollToId = model.Items.Last().Id;

            FillRecordLinks(model);

            return PartialView(model);
        }

        /// <summary>
        /// Заполнение ссылок на следующий и предыдущий блоки записей, не для режима следующие N / предыдущие N
        /// </summary>
        /// <param name="model"></param>
        private void FillRecordLinks(LogPartialModel model)
        {
            // Рассчитаем ссылку на предыдущие записи
            if (model.Items.Length > 0)
            {
                var firstRecord = model.Items[0];
                model.Previous = new LogPartialModel.DateAndOrderModel()
                {
                    Date = firstRecord.Date,
                    Order = firstRecord.Order
                };
            }
            else if (model.Filters.Date.HasValue)
            {
                model.Previous = new LogPartialModel.DateAndOrderModel()
                {
                    Date = model.Filters.Date.Value,
                    Order = 0
                };
            }
            else
                model.Previous = null;

            // Рассчитаем ссылку на следующие записи
            if (model.Items.Length > 0)
            {
                var lastRecord = model.Items.Last();
                model.Next = new LogPartialModel.DateAndOrderModel()
                {
                    Date = lastRecord.Date,
                    Order = lastRecord.Order
                };
            }
            else if (model.Filters.Date.HasValue)
            {
                model.Next = new LogPartialModel.DateAndOrderModel()
                {
                    Date = model.Filters.Date.Value,
                    Order = 0
                };
            }
            else
                model.Next = new LogPartialModel.DateAndOrderModel()
                {
                    Date = Now().AddDays(-1),
                    Order = 0
                };
        }

        /// <summary>
        /// Подгружаемая часть с блоком N предыдущих записей
        /// </summary>
        public PartialViewResult GetPreviousRecords(DateTime toDate, int order, LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            var levels = GetFilterLevels(filters.LogLevel);

            var records = GetStorage().Logs.GetPreviousRecords(filters.ComponentId.Value, toDate, order, levels, filters.Context, OutputRecordCount);

            var model = new LogPartialModel()
            {
                Filters = filters,
                Items = RecordsToModel(records),
                OutputRecordCount = OutputRecordCount
            };

            // Рассчитаем ссылку на предыдущие записи
            if (model.Items.Length > 0)
            {
                var firstRecord = model.Items[0];
                model.Previous = new LogPartialModel.DateAndOrderModel()
                {
                    Date = firstRecord.Date,
                    Order = firstRecord.Order
                };
            }
            else
            {
                model.Previous = null;
                model.NoPreviousRecords = true;
            }

            model.Next = null;
            model.MarkAsNew = true;

            if (model.Items.Length > 0)
                model.ScrollToId = model.Items.Last().Id;

            return PartialView("Partial", model);
        }

        /// <summary>
        /// Подгружаемая часть с блоком N следующих записей
        /// </summary>
        public PartialViewResult GetNextRecords(DateTime fromDate, int order, LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            var levels = GetFilterLevels(filters.LogLevel);

            var records = GetStorage().Logs.GetNextRecords(filters.ComponentId.Value, fromDate, order, levels, filters.Context, OutputRecordCount);

            var model = new LogPartialModel()
            {
                Filters = filters,
                Items = RecordsToModel(records),
                OutputRecordCount = OutputRecordCount
            };

            // Рассчитаем ссылку на следующие записи
            if (model.Items.Length > 0)
            {
                var lastRecord = model.Items.Last();
                model.Next = new LogPartialModel.DateAndOrderModel()
                {
                    Date = lastRecord.Date,
                    Order = lastRecord.Order
                };
            }
            else
            {
                model.Next = new LogPartialModel.DateAndOrderModel()
                {
                    Date = fromDate,
                    Order = order
                };
                model.NoNextRecords = true;
            }

            model.Previous = null;
            model.MarkAsNew = true;

            return PartialView("Partial", model);
        }

        /// <summary>
        /// Поиск предыдущей записи по тексту
        /// </summary>
        public JsonResult FindPreviousRecord(LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            var levels = GetFilterLevels(filters.LogLevel);

            DateTime date;
            int order;

            if (filters.Id.HasValue)
            {
                var currentRecord = GetStorage().Logs.GetOneById(filters.Id.Value);
                date = currentRecord.Date;
                order = currentRecord.Order;
            }
            else
            {
                date = filters.Date ?? Now();
                order = 0;
            }

            var logSearchResult = GetStorage().Logs.FindPreviousRecordByText(filters.Text, filters.ComponentId.Value, date, order, levels, filters.Context, SearchIterationRecordCount);

            if (!logSearchResult.Found)
            {
                if (logSearchResult.Record == null)
                    return GetSuccessJsonResponse(new
                    {
                        found = false,
                        message = "Ранее записей с таким текстом не найдено",
                        id = (Guid?)null
                    });

                return GetSuccessJsonResponse(new
                {
                    found = false,
                    id = logSearchResult.Record.Id.ToString(),
                    date = DateTimeHelper.GetRussianDateTime(logSearchResult.Record.Date)
                });
            }

            return GetSuccessJsonResponse(new
            {
                found = true,
                id = logSearchResult.Record.Id.ToString()
            });
        }

        /// <summary>
        /// Поиск следующей записи по тексту
        /// </summary>
        public JsonResult FindNextRecord(LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            var levels = GetFilterLevels(filters.LogLevel);

            DateTime date;
            int order;

            if (filters.Id.HasValue)
            {
                var currentRecord = GetStorage().Logs.GetOneById(filters.Id.Value);
                date = currentRecord.Date;
                order = currentRecord.Order;
            }
            else
            {
                date = filters.Date ?? Now();
                order = 0;
            }

            var logSearchResult = GetStorage().Logs.FindNextRecordByText(filters.Text, filters.ComponentId.Value, date, order, levels, filters.Context, SearchIterationRecordCount);

            if (!logSearchResult.Found)
            {
                if (logSearchResult.Record == null)
                    return GetSuccessJsonResponse(new
                    {
                        found = false,
                        message = "Далее записей с таким текстом не найдено",
                        id = (Guid?)null
                    });

                return GetSuccessJsonResponse(new
                {
                    found = false,
                    id = logSearchResult.Record.Id.ToString(),
                    date = DateTimeHelper.GetRussianDateTime(logSearchResult.Record.Date)
                });
            }

            return GetSuccessJsonResponse(new
            {
                found = true,
                id = logSearchResult.Record.Id.ToString()
            });
        }

        public PartialViewResult Center(LogFiltersModel filters)
        {
            if (!filters.ComponentId.HasValue)
                return null;

            if (!filters.Id.HasValue)
                return null;

            var levels = GetFilterLevels(filters.LogLevel);

            var logRecord = GetStorage().Logs.GetOneById(filters.Id.Value);

            var previousRecords = GetStorage().Logs.GetPreviousRecords(filters.ComponentId.Value, logRecord.Date, logRecord.Order, levels, filters.Context, OutputRecordCount / 2);
            var previousRecordsModel = RecordsToModel(previousRecords);

            var nextRecords = GetStorage().Logs.GetNextRecords(filters.ComponentId.Value, logRecord.Date, logRecord.Order, levels, filters.Context, OutputRecordCount - previousRecordsModel.Length - 1);
            var nextRecordsModel = RecordsToModel(nextRecords);

            var logRecordModel = RecordsToModel(new[] { logRecord });

            var model = new LogPartialModel()
            {
                Filters = filters,
                Items = previousRecordsModel.Concat(logRecordModel).Concat(nextRecordsModel).ToArray(),
                ScrollToId = logRecord.Id,
                OutputRecordCount = OutputRecordCount
            };

            FillRecordLinks(model);

            if (!string.IsNullOrEmpty(filters.Text))
            {
                var textLowercase = filters.Text.ToLower();
                var properties = GetStorage().LogProperties.GetByLogId(logRecord.Id);
                if (!logRecord.Message.ToLower().Contains(textLowercase) && properties.Any(t => t.Name.ToLower().Contains(textLowercase) || t.Value.ToLower().Contains(textLowercase)))
                {
                    model.ExpandedProperties = GetLogRowPropertiesModel(logRecord, properties);
                    model.ExpandedProperties.Text = filters.Text;
                }
            }

            return PartialView("Partial", model);
        }

        private LogLevel[] GetFilterLevels(LogLevel? level)
        {
            if (!level.HasValue)
                return _allLevels;

            return _allLevels.Where(t => t >= level.Value).ToArray();
        }

        private static LogLevel[] _allLevels = new[]
        {
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Fatal
        };

        private LogPartialModel.ItemModel[] RecordsToModel(LogForRead[] records)
        {
            return records.Select(t => new LogPartialModel.ItemModel()
            {
                Id = t.Id,
                Date = t.Date,
                Order = t.Order,
                Level = t.Level,
                Message = t.Message
            }).ToArray();
        }

        public ActionResult GetLogRowProperties(Guid componentId, Guid logRowId)
        {
            var log = GetStorage().Logs.GetOneById(logRowId);
            var properties = GetStorage().LogProperties.GetByLogId(logRowId);
            var model = GetLogRowPropertiesModel(log, properties);
            return PartialView(model);
        }

        private LogRowPropertiesModel GetLogRowPropertiesModel(LogForRead log, LogPropertyForRead[] properties)
        {
            var model = new LogRowPropertiesModel()
            {
                LogId = log.Id,
                ComponentId = log.ComponentId,
                Context = log.Context,
                Message = log.Message,
                Items = properties.OrderBy(t => t.Name).Select(t => new LogRowPropertiesModel.LogRowPropertyItem()
                {
                    Id = t.Id,
                    Name = t.Name,
                    DataType = t.DataType,
                    Value = t.Value
                }).ToArray()
            };
            return model;
        }

        public ActionResult GetLogFile(Guid id, Guid fileId)
        {
            var properties = GetStorage().LogProperties.GetByLogId(id);
            var file = properties.SingleOrDefault(x => x.Id == fileId);
            if (file == null)
            {
                throw new HttpException(404, "Файл не найден");
            }
            var contentType = GuiHelper.GetContentType(file.Name);
            var bytes = Convert.FromBase64String(file.Value);
            return File(bytes, contentType, file.Name);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid componentId)
        {
            var service = new LogService(GetStorage());
            var config = service.GetLogConfig(componentId);
            var component = GetStorage().Components.GetOneById(componentId);

            var model = new EditLogModel
            {
                Id = componentId,
                IsFatalEnabled = config.IsFatalEnabled,
                IsErrorEnabled = config.IsErrorEnabled,
                IsWarningEnabled = config.IsWarningEnabled,
                IsInfoEnabled = config.IsInfoEnabled,
                IsDebugEnabled = config.IsDebugEnabled,
                IsTraceEnabled = config.IsTraceEnabled,
                ComponentName = component.DisplayName
            };
            return PartialView(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Edit(EditLogModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var service = new LogService(GetStorage());
            var config = service.GetLogConfig(model.Id);
            var component = GetStorage().Components.GetOneById(model.Id);

            model.ComponentName = component.DisplayName;

            var configForUpdate = config.GetForUpdate();
            configForUpdate.IsFatalEnabled.Set(model.IsFatalEnabled);
            configForUpdate.IsErrorEnabled.Set(model.IsErrorEnabled);
            configForUpdate.IsWarningEnabled.Set(model.IsWarningEnabled);
            configForUpdate.IsInfoEnabled.Set(model.IsInfoEnabled);
            configForUpdate.IsDebugEnabled.Set(model.IsDebugEnabled);
            configForUpdate.IsTraceEnabled.Set(model.IsTraceEnabled);
            configForUpdate.LastUpdateDate.Set(Now());
            GetStorage().LogConfigs.Update(configForUpdate);

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult Add(Guid componentId)
        {
            var component = GetStorage().Components.GetOneById(componentId);
            var model = new AddLogModel()
            {
                ComponentId = component.Id,
                DisplayName = component.DisplayName,
                LogDate = Now(),
                LogLevel = LogLevel.Info
            };
            return PartialView(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Add(AddLogModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var client = GetDispatcherClient();
            var response = client.SendLog(CurrentUser.AccountId, new SendLogData()
            {
                ComponentId = model.ComponentId,
                Date = model.LogDate,
                Level = model.LogLevel,
                Message = model.Message
            });
            response.Check();

            return GetSuccessJsonResponse();
        }

        /// <summary>
        /// Количество строк лога в одном отображаемом блоке
        /// Не static, потому что меняется из юнит-тестов
        /// </summary>
        public readonly int OutputRecordCount = 100;

        /// <summary>
        /// Количество строк лога в одной итерации поиска
        /// Не static, потому что меняется из юнит-тестов
        /// </summary>
        public readonly int SearchIterationRecordCount = 10000;

        // Для unit-тестов

        public LogsController() { }

        public LogsController(Guid accountId, Guid userId, int outputRecordCount, int searchIterationRecordCount) : base(accountId, userId)
        {
            OutputRecordCount = outputRecordCount;
            SearchIterationRecordCount = searchIterationRecordCount;
        }

    }
}