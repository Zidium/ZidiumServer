using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Zidium.UserAccount.Helpers
{
    public class ModelStateHelper<TModel>
    {
        private ModelStateDictionary _modelState;

        private HttpContext _httpContext;

        public ModelStateHelper(ModelStateDictionary modelState, HttpContext httpContext)
        {
            _modelState = modelState;
            _httpContext = httpContext;
        }

        public void AddErrorFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string errorMessage)
        {
            var provider = _httpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var name = provider.GetExpressionText(expression);
            _modelState.AddModelError(name, errorMessage);
        }
    }
}