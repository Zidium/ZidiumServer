using System;
using Zidium.Core;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class ComponentSelectorModel : SelectorModel
    {
        public Guid? ComponentId { get; set; }

        public bool ShowAsList { get; set; }

        public bool ShowCreateNewButton { get; set; }

        public string NewComponentFolderSystemName { get; set; }

        public string NewComponentFolderDisplayName { get; set; }

        public bool ShowFindButton { get; set; }

        /// <summary>
        /// ID выпадающего списка типа компонента, который находится НЕ в данном элементе управления
        /// См. пример на странице юнит-тестов (выбор типа компонента расположен на основной форме)
        /// </summary>
        public string ExternalComponentTypeSelectId { get; set; }

        /// <summary>
        /// ИД типа компонента в диалоговом окне "Содать новый"
        /// </summary>
        public Guid? CreateNewDialogDefualtComponentTypeId { get; set; }

        public string GetComponentTypeSelectorId()
        {
            if (ExternalComponentTypeSelectId == null)
            {
                return "createNewComponentTypeId";
            }
            return ExternalComponentTypeSelectId;
        }

        public bool ShowComponentTypeSelector { get; set; }

        public bool ShowComponentStatusSelector { get; set; }

        public string GetComponentFullName()
        {
            if (string.IsNullOrEmpty(ComponentFullName))
            {
                if (ComponentId.HasValue && ComponentId.Value != Guid.Empty)
                {
                    var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
                    var component = storage.Components.GetOneById(ComponentId.Value);
                    ComponentFullName = ShowAsList ? component.DisplayName : new ComponentService(storage).GetFullDisplayName(component);
                }
            }
            return ComponentFullName;
        }

        public string ComponentFullName { get; set; }

        public string GetComponentName()
        {
            if (string.IsNullOrEmpty(ComponentName))
            {
                if (ComponentId.HasValue)
                {
                    var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
                    var component = storage.Components.GetOneById(ComponentId.Value);
                    ComponentName = component.DisplayName;
                }
            }
            return ComponentName;
        }

        public string ComponentName { get; set; }

    }
}