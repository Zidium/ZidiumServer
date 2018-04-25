using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using GetOrCreateComponentRequestData = Zidium.Core.Api.GetOrCreateComponentRequestData;

namespace Zidium.UserAccount.Controllers
{
    public class JsonController : ContextController
    {
        [Authorize]
        public ActionResult Index(string executeJsonCommand)
        {
            try
            {
                object result = null;
                if (executeJsonCommand == "getAllComponentsForGuiSelect")
                {
                    result = GetAllComponentsForGuiSelect();
                }
                if (executeJsonCommand == "createComponent")
                {
                    result = CreateComponent();
                }
                else
                {
                    throw new Exception("Неизвестная команда " + executeJsonCommand);
                }
                var response = new
                {
                    success = true,
                    data = result
                };
                //System.Threading.Thread.Sleep(1000 * 10);
                return Json(response);
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                var response = new
                {
                    success = false,
                    error = exception.Message
                };
                return Json(response);
            }
        }

        protected new DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient("JsonController");
        }

        protected object CreateComponent()
        {
            string typeId = Request["typeId"];
            string systemName = Request["systemName"];
            string displayName = Request["displayName"];
            string folderSystemName = Request["folderSystemName"];
            string folderDisplayName = Request["folderDisplayName"];

            if (string.IsNullOrEmpty(folderSystemName))
            {
                throw new UserFriendlyException("Не указана папка для нового компонента");
            }

            if (string.IsNullOrEmpty(typeId))
            {
                throw new UserFriendlyException("Не указан тип компонента");
            }
            if (string.IsNullOrEmpty(systemName))
            {
                throw new UserFriendlyException("Не указано системное имя компонента");
            }
            Guid typeIdGuid = new Guid(typeId);
            var dispatcher = GetDispatcherClient();

            // создаем папку
            var getFolderData = new GetOrCreateComponentRequestData()
            {
                SystemName = folderSystemName,
                DisplayName = folderDisplayName,
                TypeId = SystemComponentTypes.Folder.Id
            };
            var folderResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, getFolderData);
            if (folderResponse.Success == false)
            {
                throw new UserFriendlyException("Не удалось создать папку для нового компонента: " + folderResponse.ErrorMessage);
            }

            // создаем компонент
            var data = new GetOrCreateComponentRequestData()
            {
                DisplayName = displayName,
                SystemName = systemName,
                TypeId = typeIdGuid,
                ParentComponentId = folderResponse.Data.Component.Id
            };
            var componentResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, data);
            if (componentResponse.Success)
            {
                return new {id = componentResponse.Data.Component.Id};
            }
            throw new UserFriendlyException("Не удалось создать новый компонент: " + componentResponse.ErrorMessage);
        }

        protected object GetAllComponentsForGuiSelect()
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var components = repository.QueryAll().
                Where(t => t.ComponentTypeId != SystemComponentTypes.Folder.Id && t.ComponentTypeId != SystemComponentTypes.Root.Id && t.IsDeleted == false).OrderBy(t => t.DisplayName);
            return components.Select(x => new
                {
                    id = x.Id,
                    systemName = x.SystemName,
                    displayName = x.DisplayName,
                    typeId = x.ComponentTypeId
                })
                .ToArray();
        }
    }
}