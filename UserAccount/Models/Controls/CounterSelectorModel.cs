using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models
{
    public class CounterSelectorModel : SelectorModel
    {
        public CounterSelectorModel(string name, Guid? counterId, Guid componentId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, Expression<Func<object, object>> expression = null, IHtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            CounterId = counterId;
            ComponentId = componentId;
            AllowEmpty = allowEmpty;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
        }

        public Guid? CounterId { get; set; }

        public Guid ComponentId;

    }
}