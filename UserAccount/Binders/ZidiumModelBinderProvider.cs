using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zidium.UserAccount;
using Zidium.UserAccount.Binders;
using Zidium.UserAccount.Models.Controls;

namespace Web.ModelBindings
{
    public class ZidiumModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
            {
                return new DateTimeBinder();
            }

            if (context.Metadata.ModelType == typeof(ColorStatusSelectorValue))
            {
                return new ColorStatusValueBinder();
            }

            if (context.Metadata.ModelType == typeof(TimeSpan) || context.Metadata.ModelType == typeof(TimeSpan?))
            {
                return new TimeSpanBinder();
            }

            if (context.Metadata.ModelType == typeof(Time) || context.Metadata.ModelType == typeof(Time?))
            {
                return new TimeBinder();
            }

            return null;
        }
    }
}
