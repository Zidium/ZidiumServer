using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models
{
    public class UnitTestTypeSelectorModel : SelectorModel
    {
        public UnitTestTypeSelectorModel(string name, Guid? unitTestTypeId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, Expression<Func<object, object>> expression = null, IHtmlHelper<object> htmlHelper = null, bool userOnly = false)
        {
            Name = name;
            UnitTestTypeId = unitTestTypeId;
            AllowEmpty = allowEmpty;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
            UserOnly = userOnly;
        }

        public Guid? UnitTestTypeId { get; set; }

        public bool UserOnly { get; set; }
    }
}