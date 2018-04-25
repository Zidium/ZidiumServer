using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount
{
    public static class ComponentSelectorHelper
    {
        public static MvcHtmlString ComponentSelector<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            return htmlHelper.Partial("~/Views/Controls/ComponentSelector.cshtml", model);
        }

        public static MvcHtmlString ComponentSelector(
            this HtmlHelper<object> htmlHelper,
            Expression<Func<object, object>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            model.HtmlHelper = htmlHelper;
            model.Expression = expression;
            return htmlHelper.Partial("~/Views/Controls/ComponentSelector.cshtml", model);
        }

        private static string GetName<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            return htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        }

        private static Guid? GetComponentId<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string name)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            ModelState modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Value != null)
                fromModelState = (Guid?)modelState.Value.ConvertTo(typeof(Guid?), null);
            return fromModelState ?? (Guid?)metadata.Model;
        }

        private static ComponentSelectorModel PrepareModel(string name, Guid? componentId, ComponentSelectorOptions options)
        {
            var model = new ComponentSelectorModel
            {
                Name = name,
                AccountId = UserHelper.CurrentUser.AccountId,
                ComponentId = componentId,
                AllowEmpty = options.AllowEmpty,
                ShowAsList = options.ShowAsList,
                AutoRefreshPage = options.AutoRefreshPage,
                HideWhenFilter = options.HideWhenFilter,
                ExternalComponentTypeSelectId = options.ExternalComponentTypeSelectId,
                ShowCreateNewButton = options.ShowCreateButton,
                ShowFindButton = options.ShowFindButton,
                ComponentFullName = options.ComponentName,
                CreateNewDialogDefualtComponentTypeId = options.CreateNewDialogDefaultComponentTypeId,
                NewComponentFolderSystemName = options.NewComponentFolderSystemName,
                NewComponentFolderDisplayName = options.NewComponentFolderDisplayName,
                ShowComponentStatusSelector = options.ShowComponentStatusSelector,
                ShowComponentTypeSelector = options.ShowComponentTypeSelector
            };
            return model;
        }

        public static MvcHtmlString ComponentSelectorNew<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            return htmlHelper.Partial("~/Views/Controls/ComponentSelectorNew.cshtml", model);
        }

    }
}