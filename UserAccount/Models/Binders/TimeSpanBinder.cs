using System;
using System.Web.Mvc;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Controllers;

namespace Zidium.UserAccount.Binders
{
    public class TimeSpanBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == null)
                return null;

            var value = valueResult.ConvertTo(typeof(string)) as string;

            if (string.IsNullOrEmpty(value))
                return null;

            var controller = (ContextController) controllerContext.Controller;
            try
            {
                return TimeSpanHelper.ParseHtml(value);
            }
            catch (Exception)
            {
                controller.ModelState.AddModelError(bindingContext.ModelName, "Неверный формат");
                controller.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
                return null;
            }
        }
    }
}