using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupBuider<TModel> where TModel : class
    {
        public string GroupTemplate { get; set; }
        protected IHtmlHelper<TModel> HtmlHelper { get; set; }
        protected TModel Model { get; set; }

        public static FormGroupBuider<TModel> GetDefault(IHtmlHelper<TModel> htmlHelper, TModel model)
        {
            return new FormGroupBuider<TModel>()
            {
                GroupTemplate = "SmartFormGroup",
                HtmlHelper = htmlHelper,
                Model = model
            };
        }

        private static ModelExpressionProvider GetExpressionProvider(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
        }

        public FormGroupRender<TModel> Group(string name)
        {
            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model);
        }

        public FormGroupRender<TModel> GroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var name = GetExpressionProvider(HtmlHelper).GetExpressionText(expression);
            var displayName = HtmlHelper.DisplayNameFor(expression)?.ToString();
            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model, displayName);
        }

        public FormGroupRender<TModel> GroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string labelTile)
        {
            var name = GetExpressionProvider(HtmlHelper).GetExpressionText(expression);

            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model)
                .Label(labelTile);
        }
    }
}