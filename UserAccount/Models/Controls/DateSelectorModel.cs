using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models
{
    public class DateSelectorModel : SelectorModel
    {
        public DateTime? Date { get; set; }

        public DateSelectorModel(string name, DateTime? date, bool autoRefreshPage, bool hideWhenFilter = false, Expression<Func<object, object>> expression = null, IHtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            Date = date;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
        }
    }
}