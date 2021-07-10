using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Binders
{
    public class ColorStatusValueBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var colorValue = new ColorStatusSelectorValue();
            if (valueResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Success(colorValue);
                return Task.CompletedTask;
            }

            var value = valueResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(colorValue);
                return Task.CompletedTask;
            }

            try
            {
                value = value.ToLowerInvariant();
                colorValue.RedChecked = value.Contains("red");
                colorValue.YellowChecked = value.Contains("yellow");
                colorValue.GreenChecked = value.Contains("green");
                colorValue.GrayChecked = value.Contains("gray");
                bindingContext.Result = ModelBindingResult.Success(colorValue);
            }
            catch (FormatException)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Неверный формат");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            }

            return Task.CompletedTask;
        }
    }
}