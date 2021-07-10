using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Zidium.UserAccount.Helpers
{
    public class ModelHelper<TModel>
    {
        private TModel _model;

        public ModelHelper(TModel model)
        {
            _model = model;
        }

        public string GetPropertyPath<TProperty>(IHtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return GetExpressionProvider(htmlHelper).GetExpressionText(expression);
        }

        private ModelExpressionProvider GetExpressionProvider(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
        }

    }
}