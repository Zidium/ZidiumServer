using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models
{
    public class SelectorModel
    {
        public string Name { get; set; }

        public bool AllowEmpty { get; set; }

        public bool AutoRefreshPage { get; set; }

        public bool HideWhenFilter { get; set; }

        public Expression<Func<object, object>> Expression { get; set; }

        public HtmlHelper<object> HtmlHelper { get; set; }

        public HtmlHelper HtmlHelperBase { get; set; }
    }
}