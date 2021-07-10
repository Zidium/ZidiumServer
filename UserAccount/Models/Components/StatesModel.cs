using System;
using System.Collections.Generic;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Components
{
    public class StatesModel
    {
        public StatesModel()
        {
            Color = new ColorStatusSelectorValue();
            Rows = new List<StatesModelRow>();
        }

        public Guid? ComponentTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public Guid? ComponentId { get; set; }

        public Guid? ParentComponentId { get; set; }

        public string SearchString { get; set; }

        /// <summary>
        /// Если True - показывает статистику по всем цветам, 
        /// Если False - показывает только самый опасный цвет
        /// </summary>
        public bool AllColors { get; set; }

        public List<StatesModelRow> Rows { get; set; }

        public bool IsFilterEmpty()
        {
            return ParentComponentId == null 
                && ComponentTypeId == null 
                && ComponentId == null 
                && Color.NotChecked 
                && SearchString == null;
        }
    }
}