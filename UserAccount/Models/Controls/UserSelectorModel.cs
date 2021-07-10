using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UserSelectorModel : SelectorModel
    {
        public UserSelectorModel(string name, Guid? userId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, Expression<Func<object, object>> expression = null, IHtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            UserId = userId;
            AllowEmpty = allowEmpty;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
        }

        public Guid? UserId { get; set; }

        public void Update(UserSelectorForOptions options)
        {
            if (options == null)
            {
                return;
            }
            AllowEmpty = options.AllowEmpty;
        }
    }
}