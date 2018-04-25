using System;
using System.Web.Mvc;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Binders
{
    public class ColorStatusValueBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var colorValue = new ColorStatusSelectorValue();
            if (valueResult == null)
            {
                return colorValue;
            }

            var value = valueResult.ConvertTo(typeof(string)) as string;
            if (string.IsNullOrWhiteSpace(value))
            {
                //if (bindingContext.ModelType.IsGenericType && .GetGenericTypeDefinition() == typeof(Nullable<>))
                // что нужно делать, если тип модели не nullable не понятно
                return colorValue;
            }

            try
            {
                value = value.ToLowerInvariant();
                colorValue.RedChecked = value.Contains("red");
                colorValue.YellowChecked = value.Contains("yellow");
                colorValue.GreenChecked = value.Contains("green");
                colorValue.GrayChecked = value.Contains("gray");
                return colorValue;
            }
            catch (FormatException)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Неверный формат");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
                return new ColorStatusSelectorValue();
            }
        }
    }
}