using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models.Others
{
    public class ModelStateHelper<TModel>
    {
        private ModelStateDictionary _modelState;

        public ModelStateHelper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public void AddErrorFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string errorMessage)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            _modelState.AddModelError(name, errorMessage);
        }
    }
}