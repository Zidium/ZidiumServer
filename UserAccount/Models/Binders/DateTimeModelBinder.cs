using System;
using System.Web.Mvc;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Binders
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null)
            {
                return null;
            }

            var value = valueResult.ConvertTo(typeof(string)) as string;
            if (string.IsNullOrWhiteSpace(value))
            {
                //if (bindingContext.ModelType.IsGenericType && .GetGenericTypeDefinition() == typeof(Nullable<>))
                // что нужно делать, если тип модели не nullable не понятно
                return null;
            }
            try
            {
                return DateTimeHelper.FromUrlFormat(value);
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Неверный формат");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
                return null;
            }
        }
    }
}