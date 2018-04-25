using System;

namespace Zidium.UserAccount.Models.Controls
{
    public class ComponentSelectorOptions
    {
        public string ExternalComponentTypeSelectId { get; set; }

        public bool AutoRefreshPage { get; set; }

        public bool ShowAsList { get; set; }

        public bool HideWhenFilter { get; set; }

        public bool AllowEmpty { get; set; }

        public bool ShowCreateButton { get; protected set; }

        public string NewComponentFolderSystemName { get; protected set; }

        public string NewComponentFolderDisplayName { get; protected set; }

        /// <summary>
        /// ИД типа компонента по умолчанию в диалоговом окне "Создать новый"
        /// </summary>
        public Guid? CreateNewDialogDefaultComponentTypeId { get; set; }

        public bool ShowFindButton { get; set; }

        public string ComponentName { get; set; }

        public bool ShowComponentStatusSelector { get; set; }

        public bool ShowComponentTypeSelector { get; set; }

        public ComponentSelectorOptions()
        {
            AutoRefreshPage = false;
            ShowAsList = true;
            HideWhenFilter = false;
            AllowEmpty = false;
            ShowCreateButton = false;
        }

        public ComponentSelectorOptions SetShowCreateNewButton(
            Guid? defaultComponentTypeId, 
            string folderSystemName, 
            string folderDisplayName)
        {
            ShowCreateButton = true;
            CreateNewDialogDefaultComponentTypeId = defaultComponentTypeId;
            NewComponentFolderSystemName = folderSystemName;
            NewComponentFolderDisplayName = folderDisplayName;
            return this;
        }
    }
}