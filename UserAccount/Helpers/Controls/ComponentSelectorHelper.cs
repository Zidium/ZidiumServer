using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount
{
    public static class ComponentSelectorHelper
    {
        public static IHtmlContent ComponentSelector<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            return htmlHelper.Partial("~/Views/Controls/ComponentSelector.cshtml", model);
        }

        public static IHtmlContent ComponentSelector(
            this IHtmlHelper<object> htmlHelper,
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

        private static ModelExpressionProvider GetExpressionProvider(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
        }

        private static ModelExpressionProvider GetModelExpressionProvider(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
        }

        private static string GetName<TModel, TProperty>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = GetExpressionProvider(htmlHelper).GetExpressionText(expression);
            return htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        }

        private static Guid? GetComponentId<TModel, TProperty>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string name)
        {
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.RawValue != null)
                fromModelState = Guid.Parse(modelState.RawValue.ToString());
            return fromModelState ?? (Guid?)provider.Model;
        }

        private static ComponentSelectorModel PrepareModel(string name, Guid? componentId, ComponentSelectorOptions options)
        {
            var model = new ComponentSelectorModel
            {
                Name = name,
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

        public static IHtmlContent ComponentSelectorNew<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            return htmlHelper.Partial("~/Views/Controls/ComponentSelectorNew.cshtml", model);
        }

        public static IHtmlContent ComponentSelectorNew(
            this IHtmlHelper<object> htmlHelper,
            Expression<Func<object, object>> expression,
            ComponentSelectorOptions options)
        {
            var fullHtmlFieldName = GetName(htmlHelper, expression);
            var componentId = GetComponentId(htmlHelper, expression, fullHtmlFieldName);
            options = options ?? new ComponentSelectorOptions();
            var model = PrepareModel(fullHtmlFieldName, componentId, options);
            model.HtmlHelper = htmlHelper;
            model.Expression = expression;
            return htmlHelper.Partial("~/Views/Controls/ComponentSelectorNew.cshtml", model);
        }

    }
}