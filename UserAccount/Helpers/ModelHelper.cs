using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Zidium.UserAccount.Helpers
{
    public class ModelHelper<TModel>
    {
        private TModel _model;

        public ModelHelper(TModel model)
        {
            _model = model;
        }

        public string GetPropertyPath<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return ExpressionHelper.GetExpressionText(expression);
        }
    }
}