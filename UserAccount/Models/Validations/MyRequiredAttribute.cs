using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models
{
    public class MyRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        public MyRequiredAttribute()
        {
            ErrorMessage = "Пожалуйста, заполните поле {0}";
        }

        // Client-side validation support
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "required"
            };
            return new[] { rule };
        }
    }
}