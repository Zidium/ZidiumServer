using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupBuider<TModel> where TModel : class 
    {
        public string GroupTemplate { get; set; }
        protected HtmlHelper<TModel> HtmlHelper { get; set; }
        protected TModel Model { get; set; }

        public static FormGroupBuider<TModel> GetDefault(HtmlHelper<TModel> htmlHelper, TModel model)
        {
            return new FormGroupBuider<TModel>()
            {
                GroupTemplate = "SmartFormGroup",
                HtmlHelper = htmlHelper,
                Model = model
            };
        }

        public FormGroupRender<TModel> Group(string name)
        {
            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model);
        }

        public FormGroupRender<TModel> GroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var displayName = HtmlHelper.DisplayNameFor(expression)?.ToString();
            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model, displayName);
        }

        public FormGroupRender<TModel> GroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string labelTile)
        {
            var name = ExpressionHelper.GetExpressionText(expression);

            return new FormGroupRender<TModel>(name, GroupTemplate, HtmlHelper, Model)
                .Label(labelTile);
        }

        //public MvcHtmlString Raw(string name, string label, Func<object, HelperResult> control)
        //{
        //    var controlResult = control.Invoke(null);
        //    var controlHtml = MvcHtmlString.Create(controlResult.ToHtmlString());
        //    return Raw(name, label, controlHtml);
        //}

        //public MvcHtmlString Raw(string name, string label, MvcHtmlString controlHtml)
        //{
        //    var groupModel = new FormGroupModel()
        //    {
        //        Name = name,
        //        Label = new MvcHtmlString(label),
        //        Control = controlHtml
        //    };
        //    var viewPath = $"~/Views/Controls/{GroupTemplate}.cshtml";
        //    var viewHtml = HtmlHelper.Partial(viewPath, groupModel);
        //    return viewHtml;
        //}

        //public MvcHtmlString UserSelectorFor(Expression<Func<object, Guid?>> expression, string label, UserSelectorForOptions options = null)
        //{
        //    var id = ExpressionHelper.GetExpressionText(expression);
        //    var userId = expression.Compile().Invoke(null);
        //    var model = new UserSelectorModel(id, userId, true, false);
        //    model.Update(options);
        //    var controlHtml = HtmlHelper.UserSelector(model);
        //    return Raw(id, label, controlHtml);
        //}

        //public MvcHtmlString DropDownListFor<TProperty>(
        //    string label,
        //    Expression<Func<TModel, TProperty>> expression, 
        //    IEnumerable<SelectListItem> selectList,
        //    object htmlAttributes)
        //{
        //    var id = ExpressionHelper.GetExpressionText(expression);
        //    var controlHtml = HtmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        //    return Raw(id, label, controlHtml);
        //}
    }
}