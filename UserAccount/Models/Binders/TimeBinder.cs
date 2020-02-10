using System;
using System.Web.Mvc;
using Zidium.UserAccount.Controllers;

namespace Zidium.UserAccount.Binders
{
    public class TimeBinder : IModelBinder
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
                return Time.Parse(value);
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