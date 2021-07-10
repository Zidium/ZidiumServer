using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Binders
{
    public class DateTimeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == ValueProviderResult.None)
                return Task.CompletedTask;

            var value = valueResult.FirstValue;

            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            try
            {
                bindingContext.Result = ModelBindingResult.Success(DateTimeHelper.FromUrlFormat(value));
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Неверный формат");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            }

            return Task.CompletedTask;
        }
    }
}