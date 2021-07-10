using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        public DashboardController(ILogger<DashboardController> logger) : base(logger)
        {
        }

        public ActionResult Index()
        {
            var model = new DashboardModel();
            var storage = GetStorage();

            // Компоненты - общая информация
            var components = storage.Gui.GetComponentMiniList()
                .Where(t => t.ComponentTypeId != SystemComponentType.Root.Id
                    && t.ComponentTypeId != SystemComponentType.Folder.Id).ToArray();

            model.ComponentsTotal = new DashboardComponentsInfoModel()
            {
                TotalCount = components.Count(),
                AlarmCount = components.Count(t => t.Status == MonitoringStatus.Alarm),
                WarningCount = components.Count(t => t.Status == MonitoringStatus.Warning),
                SuccessCount = components.Count(t => t.Status == MonitoringStatus.Success),
                OtherCount = components.Count(t => t.Status == MonitoringStatus.Disabled || t.Status == MonitoringStatus.Unknown)

            };

            var componentTypes =
                storage.ComponentTypes.GetMany(components.Select(t => t.ComponentTypeId).Distinct().ToArray())
                    .ToDictionary(a => a.Id, b => b);

            // Компоненты - разбивка по типам
            var componentGroups = components
                .GroupBy(t => t.ComponentTypeId)
                .Select(t => new
                {
                    ComponentType = componentTypes[t.Key],
                    Components = t
                })
                .OrderBy(t => t.ComponentType.DisplayName)
                .ToArray();

            model.ComponentsByTypes = componentGroups
                .Select(x => new DashboardComponentsInfoModel()
                {
                    ComponentType = x.ComponentType,
                    TotalCount = components.Count(),
                    AlarmCount = components.Count(t => t.Status == MonitoringStatus.Alarm),
                    WarningCount = components.Count(t => t.Status == MonitoringStatus.Warning),
                    SuccessCount = components.Count(t => t.Status == MonitoringStatus.Success),
                    OtherCount = components.Count(t => t.Status == MonitoringStatus.Disabled || t.Status == MonitoringStatus.Unknown)

                })
                .ToArray();

            return View(model);
        }
    }
}