using System;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    public class ComponentSelectorModel : SelectorModel
    {
        public Guid AccountId;

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
                    using (var databasesContext = new DatabasesContext())
                    {
                        var context = databasesContext.GetAccountDbContext(AccountId); // todo нужно брать из текущего пользователя
                        var repository = context.GetComponentRepository();
                        var component = repository.GetById(ComponentId.Value);
                        ComponentFullName = ShowAsList ? component.DisplayName : component.GetFullDisplayName();
                    }
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
                    using (var databasesContext = new DatabasesContext())
                    {
                        var context = databasesContext.GetAccountDbContext(AccountId); // todo нужно брать из текущего пользователя
                        var repository = context.GetComponentRepository();
                        var component = repository.GetById(ComponentId.Value);
                        ComponentName = component.DisplayName;
                    }
                }
            }
            return ComponentName;
        }

        public string ComponentName { get; set; }

    }
}