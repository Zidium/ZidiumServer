using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    public class EventTypeSelectorModel : SelectorModel
    {
        protected Guid AccountId;

        public EventTypeSelectorModel(string name, Guid accountId, Guid? eventTypeId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, string externalEventCategorySelectId = null, Expression<Func<object, object>> expression = null, HtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            AccountId = accountId;
            EventTypeId = eventTypeId;
            AllowEmpty = allowEmpty;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
            ExternalEventCategorySelectId = externalEventCategorySelectId;
        }

        public Guid? EventTypeId { get; set; }

        public string ExternalEventCategorySelectId { get; set; }

        public bool ShowEventCategory { get { return ExternalEventCategorySelectId == null; } }

        public string EventTypeName
        {
            get
            {
                string result = null;
                if (EventTypeId.HasValue)
                {
                    using (var databasesContext = new DatabasesContext())
                    {
                        var context = databasesContext.GetAccountDbContext(AccountId);
                        var repository = context.GetEventTypeRepository();
                        var eventType = repository.GetById(EventTypeId.Value);
                        if (eventType != null)
                        {
                            result = eventType.DisplayName;
                        }
                    }
                }
                return result;
            }
        }

    }
}