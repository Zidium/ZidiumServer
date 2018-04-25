using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class EnumSelectorModel : SelectorModel
    {
        public EnumSelectorModel(
            string name, 
            object value, 
            Type type, 
            bool autoRefreshPage, 
            bool hideWhenFilter = false, 
            Expression<Func<object, object>> expression = null, 
            HtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            Value = value;
            AutoRefreshPage = autoRefreshPage;
            Expression = expression;
            HtmlHelper = htmlHelper;
            Type = type;
            var basetype = Nullable.GetUnderlyingType(Type);
            AllowEmpty = basetype != null;
            if (basetype != null)
            {
                Type = basetype;
            }
            HideWhenFilter = hideWhenFilter;
        }

        public object Value { get; set; }

        public Type Type { get; set; }

        public List<object> AllItems { get; set; }

        public List<SelectListItem> GetSelectListItems()
        {
            return GuiHelper.GetEnumItems(Type, Value, AllowEmpty, AllItems).ToList();
        } 
    }
}