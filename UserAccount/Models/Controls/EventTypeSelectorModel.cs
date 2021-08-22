using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class EventTypeSelectorModel : SelectorModel
    {
        public EventTypeSelectorModel(string name, Guid? eventTypeId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, string externalEventCategorySelectId = null, Expression<Func<object, object>> expression = null, IHtmlHelper<object> htmlHelper = null)
        {
            Name = name;
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
                    var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
                    var eventType = storage.EventTypes.GetOneById(EventTypeId.Value);
                    if (eventType != null)
                    {
                        result = eventType.DisplayName;
                    }
                }
                return result;
            }
        }

    }
}