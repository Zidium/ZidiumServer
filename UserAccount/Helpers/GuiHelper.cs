﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Helpers
{
    public static class GuiHelper
    {
        public static readonly string StrongRedBgColor = "#f56954";
        public static readonly string StrongYellowBgColor = "#f39c12";
        public static readonly string StrongGreenBgColor = "#00a65a";
        public static readonly string StrongGrayBgColor = "#999";

        public static readonly string StrongRedFgColor = "#a94442";
        public static readonly string StrongYellowFgColor = "#A07C00";
        public static readonly string StrongGreenFgColor = "#3c763d";
        public static readonly string StrongGrayFgColor = "#999";

        public static FormGroupBuider<TModel> GetFormBuilder<TModel>(this IHtmlHelper<TModel> htmlHelper, TModel model) where TModel : class
        {
            return FormGroupBuider<TModel>.GetDefault(htmlHelper, model);
        }

        public static ImportanceColor GetImportanceColor(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
            {
                return ImportanceColor.Red;
            }
            if (status == MonitoringStatus.Warning)
            {
                return ImportanceColor.Yellow;
            }
            if (status == MonitoringStatus.Success)
            {
                return ImportanceColor.Green;
            }
            return ImportanceColor.Gray;
        }

        public static ImportanceColor GetImportanceColor(UnitTestResult status)
        {
            if (status == UnitTestResult.Alarm)
            {
                return ImportanceColor.Red;
            }
            if (status == UnitTestResult.Warning)
            {
                return ImportanceColor.Yellow;
            }
            if (status == UnitTestResult.Success)
            {
                return ImportanceColor.Green;
            }
            return ImportanceColor.Gray;
        }

        public static string GetStrongFgColor(ImportanceColor color)
        {
            if (color == ImportanceColor.Red)
            {
                return StrongRedFgColor;
            }
            if (color == ImportanceColor.Yellow)
            {
                return StrongYellowFgColor;
            }
            if (color == ImportanceColor.Green)
            {
                return StrongGreenFgColor;
            }
            return StrongGrayFgColor;
        }

        public static string GetStrongBgColor(ImportanceColor color)
        {
            if (color == ImportanceColor.Red)
            {
                return StrongRedBgColor;
            }
            if (color == ImportanceColor.Yellow)
            {
                return StrongYellowBgColor;
            }
            if (color == ImportanceColor.Green)
            {
                return StrongGreenBgColor;
            }
            return StrongGrayBgColor;
        }

        public static string GetStrongBgColor(EventImportance color)
        {
            if (color == EventImportance.Alarm)
            {
                return StrongRedBgColor;
            }
            if (color == EventImportance.Warning)
            {
                return StrongYellowBgColor;
            }
            if (color == EventImportance.Success)
            {
                return StrongGreenBgColor;
            }
            return StrongGrayBgColor;
        }

        public static string GetStrongBgColorCss(ObjectColor status)
        {
            if (status == ObjectColor.Red)
            {
                return "text-strongbgred";
            }
            if (status == ObjectColor.Yellow)
            {
                return "text-strongbgyellow";
            }
            if (status == ObjectColor.Green)
            {
                return "text-strongbggreen";
            }
            return "text-strongbggray";
        }

        public static string GetStrongBgColorCss(ImportanceColor color)
        {
            if (color == ImportanceColor.Red)
            {
                return "text-strongbgred";
            }
            if (color == ImportanceColor.Yellow)
            {
                return "text-strongbgyellow";
            }
            if (color == ImportanceColor.Green)
            {
                return "text-strongbggreen";
            }
            return "text-strongbggray";
        }

        public static string GetStrongFgColor(ObjectColor color)
        {
            if (color == ObjectColor.Red)
            {
                return StrongRedFgColor;
            }
            if (color == ObjectColor.Yellow)
            {
                return StrongYellowFgColor;
            }
            if (color == ObjectColor.Green)
            {
                return StrongGreenFgColor;
            }
            return StrongGrayFgColor;
        }

        public static string GetStrongFgColor(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
            {
                return StrongRedFgColor;
            }
            if (status == MonitoringStatus.Warning)
            {
                return StrongYellowFgColor;
            }
            if (status == MonitoringStatus.Success)
            {
                return StrongGreenFgColor;
            }
            return StrongGrayFgColor;
        }

        public static string GetStrongFgColor(EventImportance color)
        {
            if (color == EventImportance.Alarm)
            {
                return StrongRedFgColor;
            }
            if (color == EventImportance.Warning)
            {
                return StrongYellowFgColor;
            }
            if (color == EventImportance.Success)
            {
                return StrongGreenFgColor;
            }
            return StrongGrayFgColor;
        }

        public static bool HasSummaryErrors(HttpContext httpContext)
        {
            var modelState = FullRequestContext.GetCurrent(httpContext).Controller.ModelState;
            foreach (var pair in modelState)
            {
                if (pair.Key == "" && pair.Value.Errors.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static IHtmlContent DateSelector(this IHtmlHelper htmlHelper, string name, DateTime? date, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            var model = new DateSelectorModel(name, date, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/DateSelector.cshtml", model);
        }

        private static ModelExpressionProvider GetModelExpressionProvider(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
        }

        public static IHtmlContent DateSelector(this IHtmlHelper<object> htmlHelper, Expression<Func<object, object>> expression, bool hideWhenFilter = false)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            DateTime? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.RawValue != null)
                fromModelState = DateTime.Parse(modelState.RawValue.ToString());
            var date = fromModelState ?? (DateTime?)provider.Model;
            var model = new DateSelectorModel(fullHtmlFieldName, date, false, hideWhenFilter, expression, htmlHelper);
            return htmlHelper.Partial("~/Views/Controls/DateSelector.cshtml", model);
        }

        public static IHtmlContent EventImportanceLabel(this IHtmlHelper htmlHelper, EventImportance? eventImportance)
        {
            return htmlHelper.Partial("~/Views/Controls/EventImportance.cshtml", eventImportance);
        }

        public static IHtmlContent MonitoringStatusLabel(this IHtmlHelper htmlHelper, MonitoringStatus status)
        {
            var model = new MonitoringStatusModel()
            {
                Status = status,
                FontSize = 28
            };
            return htmlHelper.Partial("~/Views/Controls/MonitoringStatus.cshtml", model);
        }

        public static IHtmlContent MonitoringStatusLabel(this IHtmlHelper htmlHelper, MonitoringStatus status, Guid statusEventId, int fontSize = 28, string text = null)
        {
            var model = new MonitoringStatusModel()
            {
                StatusEventId = statusEventId,
                Status = status,
                FontSize = fontSize,
                Text = text
            };
            return htmlHelper.Partial("~/Views/Controls/MonitoringStatus.cshtml", model);
        }

        public static IHtmlContent EventLinkStartDate(this IHtmlHelper htmlHelper, EventForRead eventObj, int timeZoneOffset)
        {
            return htmlHelper.ActionLink(DateTimeHelper.GetRussianDateTime(eventObj.StartDate, timeZoneOffset), "Show", "Events", new { id = eventObj.Id }, null);
        }

        public static IHtmlContent ComponentLink(this IHtmlHelper htmlHelper, Guid componentId, string name)
        {
            return htmlHelper.ActionLink(name, "Show", "Components", new { id = componentId }, null);
        }

        public static IHtmlContent UnitTestLink(this IHtmlHelper htmlHelper, Guid unitTestId, string name)
        {
            return htmlHelper.ActionLink(name, "ResultDetails", "UnitTests", new { id = unitTestId }, null);
        }

        public static IHtmlContent MetricLink(this IHtmlHelper htmlHelper, Guid metricId, string name)
        {
            return htmlHelper.ActionLink(name, "Show", "Metrics", new { id = metricId }, null);
        }

        public static IHtmlContent KeyValueRow(this IHtmlHelper htmlHelper, KeyValueRowModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/KeyValueRow.cshtml", model);
        }

        public static IHtmlContent KeyValueTable<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string addRowTitle)
            where TProperty : IEnumerable<KeyValueRowModel>
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var value = GetExpressionValue(htmlHelper, expression);
            var rows = value != null ? value.ToList() : new List<KeyValueRowModel>();
            rows.ForEach(x => x.CollectionName = name);
            var model = new KeyValueTableModel()
            {
                CollectionName = name,
                Rows = rows,
                AddRowButtonTitle = addRowTitle
            };
            return htmlHelper.Partial("~/Views/Controls/KeyValueTable.cshtml", model);
        }

        public static IHtmlContent ShowKeyValueTable<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
            where TProperty : IEnumerable<KeyValueRowModel>
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var value = GetExpressionValue(htmlHelper, expression);
            var rows = value != null ? value.ToList() : new List<KeyValueRowModel>();
            rows.ForEach(x => x.CollectionName = name);
            var model = new KeyValueTableModel()
            {
                CollectionName = name,
                Rows = rows
            };
            return htmlHelper.Partial("~/Views/Controls/ShowKeyValueTable.cshtml", model);
        }

        public static IHtmlContent ColorStatusSelector<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return ColorStatusSelector(htmlHelper, expression, null);
        }

        public static IHtmlContent ColorStatusSelector<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ColorStatusSelectorOptions options)
        {
            if (options == null)
            {
                options = new ColorStatusSelectorOptions();
            }
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var value = GetExpressionValue(htmlHelper, expression);
            var model = new ColorStatusSelectorModel()
            {
                PropertyName = fullHtmlFieldName,
                Value = value as ColorStatusSelectorValue,
                AutoRefreshPage = options.AutoRefreshPage,
                MultiSelectMode = options.MultiSelectMode,
                Callback = options.Callback
            };
            if (model.Value == null)
            {
                model.Value = new ColorStatusSelectorValue();
            }
            return htmlHelper.Partial("~/Views/Controls/ColorStatusSelector.cshtml", model);
        }

        public static TProperty GetExpressionValue<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            object result = null;

            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState)
                && modelState.RawValue != null)
            {
                result = (TProperty)modelState.RawValue;
            }

            result = result ?? (TProperty)provider.Model;
            return (TProperty)result;
        }

        public static IHtmlContent ColorCircleWithNumber(this IHtmlHelper htmlHelper, int value, ImportanceColor color, string url)
        {
            var model = new ColorCircleWithNumberModel()
            {
                Color = color,
                Value = value,
                Url = url
            };
            return ColorCircleWithNumber(htmlHelper, model);
        }

        public static IHtmlContent ColorCircleWithNumber(this IHtmlHelper htmlHelper, ColorCircleWithNumberModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/ColorCircleWithNumber.cshtml", model);
        }

        public static IHtmlContent BigColorBlock(this IHtmlHelper htmlHelper, BigColorBlockModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/BigColorBlock.cshtml", model);
        }

        public static IHtmlContent StatusByTypeTable(this IHtmlHelper htmlHelper, StatusByTypeTableModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/StatusByTypeTable.cshtml", model);
        }

        public static IHtmlContent ExtentionPropertiesTable(this IHtmlHelper htmlHelper, ExtentionPropertiesModel model)
        {
            return htmlHelper.Partial("~/Views/ExtentionProperties/ExtentionPropertiesTable.cshtml", model);
        }

        public static IHtmlContent ExtentionPropertiesTable(this IHtmlHelper htmlHelper, EventPropertyForRead[] properties)
        {
            var model = ExtentionPropertiesModel.Create(properties);
            return ExtentionPropertiesTable(htmlHelper, model);
        }

        public static IHtmlContent ExtentionPropertiesTable(this IHtmlHelper htmlHelper, ComponentPropertyForRead[] properties)
        {
            var model = ExtentionPropertiesModel.Create(properties);
            return ExtentionPropertiesTable(htmlHelper, model);
        }

        private static MonitoringStatus GetComponentStatus(ImportanceColor color)
        {
            if (color == ImportanceColor.Red)
            {
                return MonitoringStatus.Alarm;
            }
            if (color == ImportanceColor.Yellow)
            {
                return MonitoringStatus.Warning;
            }
            if (color == ImportanceColor.Green)
            {
                return MonitoringStatus.Success;
            }
            return MonitoringStatus.Unknown;
        }

        public static IHtmlContent ComponentTypeSelector(
            this IHtmlHelper htmlHelper,
            string name,
            Guid? componentTypeId,
            bool allowEmpty,
            bool autoRefreshPage,
            bool hideWhenFilter = false)
        {
            var model = new ComponentTypeSelectorModel(name, componentTypeId, allowEmpty, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/ComponentTypeSelector.cshtml", model);
        }

        public static IHtmlContent ComponentTypeSelectorNew<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            ComponentTypeSelectorOptions options)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            if (options == null)
            {
                options = new ComponentTypeSelectorOptions();
            }
            if (options.SelectedValue == null)
            {
                TProperty value = GetExpressionValue(htmlHelper, expression);
                options.SelectedValue = value as Guid?;
            }
            var model = new ComponentTypeSelectorModel(htmlHelper, name, options);
            return htmlHelper.Partial("~/Views/Controls/ComponentTypeSelector.cshtml", model);
        }

        public static IHtmlContent ComponentTypeSelector(
            this IHtmlHelper<object> htmlHelper,
            Expression<Func<object, object>> expression,
            bool hideWhenFilter = false)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = Guid.Parse(modelState.RawValue.ToString());
            var componentTypeId = fromModelState ?? (Guid?)provider.Model;
            var baseType = Nullable.GetUnderlyingType(provider.Metadata.ModelType);
            var model = new ComponentTypeSelectorModel(fullHtmlFieldName, componentTypeId, baseType != null, false, hideWhenFilter, expression, htmlHelper);
            return htmlHelper.Partial("~/Views/Controls/ComponentTypeSelector.cshtml", model);
        }

        public static IHtmlContent EventTypeSelector(this IHtmlHelper htmlHelper, string name, Guid? eventTypeId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false, string externalEventCategorySelectId = null)
        {
            var model = new EventTypeSelectorModel(name, eventTypeId, allowEmpty, autoRefreshPage, hideWhenFilter, externalEventCategorySelectId);
            return htmlHelper.Partial("~/Views/Controls/EventTypeSelector.cshtml", model);
        }

        public static IHtmlContent EventTypeSelector(this IHtmlHelper<object> htmlHelper, Expression<Func<object, object>> expression, bool hideWhenFilter = false, string externalEventCategorySelectId = null)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = Guid.Parse(modelState.RawValue.ToString());
            var eventTypeId = fromModelState ?? (Guid?)provider.Model;
            var baseType = Nullable.GetUnderlyingType(provider.Metadata.ModelType);
            var model = new EventTypeSelectorModel(fullHtmlFieldName, eventTypeId, baseType != null, false, hideWhenFilter, externalEventCategorySelectId, expression, htmlHelper);
            return htmlHelper.Partial("~/Views/Controls/EventTypeSelector.cshtml", model);
        }

        public static IHtmlContent CounterSelector(this IHtmlHelper htmlHelper, string name, Guid? counterId, Guid componentId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            var model = new CounterSelectorModel(name, counterId, componentId, allowEmpty, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/CounterSelector.cshtml", model);
        }

        public static IHtmlContent UserSelector(this IHtmlHelper htmlHelper, string name, Guid? userId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            var model = new UserSelectorModel(name, userId, allowEmpty, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/UserSelector.cshtml", model);
        }

        public static IHtmlContent UserSelector(this IHtmlHelper htmlHelper, UserSelectorModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/UserSelector.cshtml", model);
        }

        public static IHtmlContent UserSelector(this IHtmlHelper<object> htmlHelper, Expression<Func<object, object>> expression, bool hideWhenFilter = false)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = Guid.Parse(modelState.RawValue.ToString());
            var userId = fromModelState ?? (Guid?)provider.Model;
            var baseType = Nullable.GetUnderlyingType(provider.Metadata.ModelType);
            var model = new UserSelectorModel(fullHtmlFieldName, userId, baseType != null, false, hideWhenFilter, expression, htmlHelper);
            return htmlHelper.Partial("~/Views/Controls/UserSelector.cshtml", model);
        }

        public static IHtmlContent UnitTestTypeSelector(this IHtmlHelper htmlHelper, string name, Guid? unitTestTypeId, bool allowEmpty, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            var model = new UnitTestTypeSelectorModel(name, unitTestTypeId, allowEmpty, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/UnitTestTypeSelector.cshtml", model);
        }

        public static IHtmlContent UnitTestTypeSelector(this IHtmlHelper<object> htmlHelper, Expression<Func<object, object>> expression, bool hideWhenFilter = false, bool userOnly = false)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            Guid? fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = Guid.Parse(modelState.RawValue.ToString());
            var unitTestTypeId = fromModelState ?? (Guid?)provider.Model;
            var baseType = Nullable.GetUnderlyingType(provider.Metadata.ModelType);
            var model = new UnitTestTypeSelectorModel(fullHtmlFieldName, unitTestTypeId, baseType != null, false, hideWhenFilter, expression, htmlHelper, userOnly);
            return htmlHelper.Partial("~/Views/Controls/UnitTestTypeSelector.cshtml", model);
        }

        public static IHtmlContent EnumSelector(
            this IHtmlHelper htmlHelper,
            string name,
            object value,
            Type type,
            bool autoRefreshPage,
            bool hideWhenFilter = false,
            IEnumerable allItems = null)
        {
            var model = new EnumSelectorModel(name, value, type, autoRefreshPage, hideWhenFilter);
            if (allItems != null)
            {
                model.AllItems = allItems.AsQueryable().Cast<object>().ToList();
            }
            return htmlHelper.Partial("~/Views/Controls/EnumSelector.cshtml", model);
        }

        public static IHtmlContent EnumSelector(
            this IHtmlHelper<object> htmlHelper,
            Expression<Func<object, object>> expression,
            bool hideWhenFilter = false)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            object fromModelState = null;

            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = modelState.RawValue;

            var value = fromModelState ?? provider.Model;
            var model = new EnumSelectorModel(fullHtmlFieldName, value, provider.Metadata.ModelType, false, hideWhenFilter, expression, htmlHelper);
            return htmlHelper.Partial("~/Views/Controls/EnumSelector.cshtml", model);
        }

        public static IHtmlContent TimeSpanSelector<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            string fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = modelState.RawValue.ToString();
            var value = fromModelState ?? TimeSpanAsShortString((TimeSpan?)provider.Model);
            var model = new TimeSpanSelectorModel()
            {
                HtmlHelper = htmlHelper,
                Name = name,
                Value = value
            };
            return htmlHelper.Partial("~/Views/Controls/TimeSpanSelector.cshtml", model);
        }

        public static IHtmlContent ComponentStatusSelector(this IHtmlHelper htmlHelper, string name, MonitoringStatus[] statuses, bool autoRefreshPage, bool hideWhenFilter = false)
        {
            var model = new ComponentStatusSelectorModel(name, statuses, autoRefreshPage, hideWhenFilter);
            return htmlHelper.Partial("~/Views/Controls/ComponentStatusSelector.cshtml", model);
        }

        public static IHtmlContent RequiredLabel(this IHtmlHelper htmlHelper, string title)
        {
            return RequiredLabel(htmlHelper, title, null, null);
        }

        public static IHtmlContent RequiredLabel(this IHtmlHelper htmlHelper, string title, string forValue)
        {
            return RequiredLabel(htmlHelper, title, forValue, null);
        }

        public static IHtmlContent RequiredLabel(this IHtmlHelper htmlHelper, string title, string forValue, string cssClass)
        {
            var model = new RequiredLabelModel()
            {
                Title = title,
                For = forValue,
                CssClass = cssClass
            };
            return htmlHelper.Partial("~/Views/Controls/RequiredLabel.cshtml", model);
        }

        public static IHtmlContent RequiredLabelFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string title = null)
        {
            return RequiredLabelFor(htmlHelper, expression, title, null);
        }

        public static IHtmlContent RequiredLabelFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string title,
            string cssClass)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            if (title == null)
            {
                var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
                title = provider.Metadata.DisplayName;
            }
            return RequiredLabel(htmlHelper, title, name, cssClass);
        }

        public static IHtmlContent FilterButton(this IHtmlHelper htmlHelper, string filterElementId)
        {
            return htmlHelper.Partial("~/Views/Controls/FilterButton.cshtml", filterElementId);
        }

        public static IHtmlContent BooleanButton(this IHtmlHelper htmlHelper, string name, string captionTrue, string captionFalse, bool falseIsNull, bool value, bool autoRefreshPage)
        {
            var model = new BooleanButtonModel()
            {
                Name = name,
                AutoRefreshPage = autoRefreshPage,
                CaptionTrue = captionTrue,
                CaptionFalse = captionFalse,
                FalseIsNull = falseIsNull,
                Value = value
            };
            return htmlHelper.Partial("~/Views/Controls/BooleanButton.cshtml", model);
        }

        public static IHtmlContent TextFilter(this IHtmlHelper htmlHelper, string name, string placeholder, string value, bool autoRefreshPage)
        {
            var model = new TextFilterModel()
            {
                Name = name,
                Placeholder = placeholder,
                AutoRefreshPage = autoRefreshPage,
                Value = value
            };
            return htmlHelper.Partial("~/Views/Controls/TextFilter.cshtml", model);
        }

        public static IHtmlContent MyValidationSummary(this IHtmlHelper htmlHelper, bool noGroup = false)
        {
            if (HasSummaryErrors(htmlHelper.ViewContext.HttpContext))
            {
                var errorsHtml = htmlHelper.ValidationSummary(true);
                string html = string.Empty;
                if (!noGroup)
                    html += "<div class='form-group'><div class='col-sm-12'>";
                var text = htmlHelper.HtmlBlock(x => errorsHtml);
                html += "<div class='alert alert-danger'>" + text + "</div>";
                if (!noGroup)
                    html += "</div></div>";
                return new HtmlString(html);
            }
            return null;
        }

        public static IHtmlContent MyValidationSummaryVertical(this IHtmlHelper htmlHelper)
        {
            if (HasSummaryErrors(htmlHelper.ViewContext.HttpContext))
            {
                IHtmlContent errorsHtml = htmlHelper.ValidationSummary(true);
                string html = "<div class='form-group'><div class='col-sm-12'><div class='alert alert-danger'>" + errorsHtml + "</div></div></div>";
                return new HtmlString(html);
            }
            return null;
        }

        public static IHtmlContent TimeSelector<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = GetModelExpressionProvider(htmlHelper).GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var provider = GetModelExpressionProvider(htmlHelper).CreateModelExpression(htmlHelper.ViewData, expression);
            ModelStateEntry modelState;
            string fromModelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.RawValue != null)
                fromModelState = modelState.RawValue.ToString();
            var value = fromModelState ?? provider.Model?.ToString();
            var model = new TimeSelectorModel()
            {
                HtmlHelper = htmlHelper,
                Name = name,
                Value = value
            };
            return htmlHelper.Partial("~/Views/Controls/TimeSelector.cshtml", model);
        }

        public static void FixKeyValueRows(List<KeyValueRowModel> rows)
        {
            if (rows == null)
            {
                return;
            }
            rows.ForEach(x => x.Trim());
            rows.RemoveAll(x => x.HasKey == false);
        }

        public static string GetContentType(string file)
        {
            string ext = Path.GetExtension(file);
            // text
            if (string.Equals(ext, ".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                return "text/plain";
            }
            if (string.Equals(ext, ".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return "text/xml";
            }
            // application
            if (string.Equals(ext, ".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                return "application/pdf";
            }
            // image
            if (string.Equals(ext, ".gif", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/gif";
            }
            if (string.Equals(ext, ".jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/jpeg";
            }
            if (string.Equals(ext, ".jpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/jpeg";
            }
            if (string.Equals(ext, ".png", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/png";
            }
            return "application/octet-stream"; // двоичный файл без указания формата
        }

        public static string GetLogLevelAlertCssClass(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
            {
                return "label label-default";
            }
            if (logLevel == LogLevel.Warning)
            {
                return "label label-info";
            }
            if (logLevel == LogLevel.Info)
            {
                return "label label-primary";
            }
            if (logLevel == LogLevel.Error)
            {
                return "label label-danger";
            }
            if (logLevel == LogLevel.Fatal)
            {
                return "label label-danger";
            }
            return "label label-default";
        }

        public static string GetLogLevelTextCssClass(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Trace)
            {
                return "text-strongfggray";
            }
            if (logLevel == LogLevel.Debug)
            {
                return "text-strongfggray";
            }
            if (logLevel == LogLevel.Warning)
            {
                return "text-strongfgyellow";
            }
            if (logLevel == LogLevel.Info)
            {
                return "text-primary";
            }
            if (logLevel == LogLevel.Error)
            {
                return "text-strongfgred";
            }
            if (logLevel == LogLevel.Fatal)
            {
                return "text-strongfgred log-message-fatal";
            }
            return string.Empty;
        }

        public static string GetComponentStatusLabelCssClass(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
                return "label label-danger";
            if (status == MonitoringStatus.Warning)
                return "label label-warning";
            if (status == MonitoringStatus.Success)
                return "label label-success";
            if (status == MonitoringStatus.Unknown || status == MonitoringStatus.Disabled)
                return "label label-default";
            return string.Empty;
        }

        public static string GetEventImportanceLabelCssClass(EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
                return "label label-danger";
            if (importance == EventImportance.Warning)
                return "label label-warning";
            if (importance == EventImportance.Success)
                return "label label-info";
            if (importance == EventImportance.Unknown)
                return "label label-default";
            return string.Empty;
        }

        public static string GetEventImportanceTextCssClass(EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
                return "text-strongfgred";
            if (importance == EventImportance.Warning)
                return "text-strongfgyellow";
            return string.Empty;
        }

        public static string GetEventImportanceColorCss(EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
                return "text-strongfgred";
            if (importance == EventImportance.Warning)
                return "text-strongfgyellow";
            if (importance == EventImportance.Success)
                return "text-strongfggreen";
            if (importance == EventImportance.Unknown)
                return "text-muted";
            return "text-normal";
        }

        public static string GetEventCategoryLabelCssClass(EventCategory category)
        {
            if (category == EventCategory.ApplicationError)
                return "label label-danger";

            if (category == EventCategory.ComponentEvent)
                return "label label-success";

            if (category == EventCategory.UnitTestResult)
                return "label label-info";

            if (category == EventCategory.ComponentExternalStatus)
                return "label label-default";

            return string.Empty;
        }

        public static string GetEventTypeDisplayNameWithCode(EventTypeForRead eventType)
        {
            return (!string.IsNullOrEmpty(eventType.Code) ? eventType.Code + " - " : "") + eventType.DisplayName;
        }


        public static IHtmlContent GetMetricValueHtml(double? value, ObjectColor color, bool hasSignal)
        {
            var valueText = "null";
            if (value.HasValue)
            {
                valueText = value.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (hasSignal == false)
            {
                valueText = "нет сигнала";
            }
            var css = GetStrongBgColorCss(color);
            return new HtmlString("<span class='badge " + css + "'>" + valueText + "</span>");
        }

        public static string GetComponentStatusTextCssClass(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
                return "text-strongfgred";
            if (status == MonitoringStatus.Warning)
                return "text-strongfgyellow";
            if (status == MonitoringStatus.Success)
                return "text-strongfggreen";
            if (status == MonitoringStatus.Disabled)
                return "text-strongfggray";
            return "text-strongfggray";
        }

        public static string GetUnitTestStatusLabelCssClass(UnitTestResult? status)
        {
            if (status == UnitTestResult.Alarm)
                return "label label-danger";

            if (status == UnitTestResult.Warning)
                return "label label-warning";

            if (status == UnitTestResult.Success)
                return "label label-success";

            return "label label-default";
        }

        public static string GetUnitTestStatusTableTextCssClass(MonitoringStatus? status)
        {
            if (status == MonitoringStatus.Alarm)
                return "text-danger";

            if (status == MonitoringStatus.Warning)
                return "text-warning";

            return string.Empty;
        }

        public static string GetMonitoringStatusLabelCssClass(MonitoringStatus? status)
        {
            if (status == MonitoringStatus.Alarm)
                return "label text-strongbgred";

            if (status == MonitoringStatus.Warning)
                return "label text-strongbgyellow";

            if (status == MonitoringStatus.Success)
                return "label text-strongbggreen";

            return "label text-strongbggray";
        }

        public static List<SelectListItem> GetComponentTypes(Guid? selected, bool allowEmpty)
        {
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "Любой"
                });
            }

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var types = storage.ComponentTypes.Filter(null, int.MaxValue).Where(t => t.Id != SystemComponentType.Root.Id);

            items.AddRange(types.Select(componentType => new SelectListItem()
            {
                Selected = componentType.Id == selected,
                Value = componentType.Id.ToString(),
                Text = componentType.DisplayName
            }));
            return items;
        }

        public static List<SelectListItem> GetUsedComponentTypes(Guid? selected, bool allowEmpty)
        {
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "Любой"
                });
            }
            // Отображаем только те типы, для которых есть хотя бы один компонент
            // Кроме папок и корня
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();

            var componentTypeIds = storage.Gui.GetSimplifiedComponentList().Select(t => t.ComponentTypeId).Distinct().ToArray();

            var types = storage.ComponentTypes.GetMany(componentTypeIds)
                .Where(t => t.IsDeleted == false && t.Id != SystemComponentType.Folder.Id && t.Id != SystemComponentType.Root.Id)
                .OrderBy(t => t.DisplayName);

            items.AddRange(types.Select(componentType => new SelectListItem()
            {
                Selected = componentType.Id == selected,
                Value = componentType.Id.ToString(),
                Text = componentType.DisplayName
            }));
            return items;
        }

        public static List<SelectListItem> GetAccountEventTypes(Guid? eventTypeId, bool allowEmpty)
        {
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "Любой",
                    Selected = eventTypeId == null
                });
            }

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var eventTypes = storage.EventTypes.Filter(null, null, null, int.MaxValue);

            items.AddRange(eventTypes.Select(t => new SelectListItem()
            {
                Value = t.Id.ToString(),
                Text = t.DisplayName,
                Selected = t.Id == eventTypeId
            }));
            return items;
        }

        public static List<SelectListItem> GetAccountUsers(Guid? userId, bool allowEmpty)
        {
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = string.Empty,
                    Selected = userId == null
                });
            }

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();

            var users = storage.Users.GetAll()
                .OrderBy(t => t.Login);

            items.AddRange(users.Select(t => new SelectListItem()
            {
                Value = t.Id.ToString(),
                Text = t.NameOrLogin(),
                Selected = t.Id == userId
            }));
            return items;
        }

        public static List<SelectListItem> GetUnitTestTypes(Guid? unitTestTypeId, bool allowEmpty, bool userOnly = false)
        {
            var items = new List<SelectListItem>();

            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "Все",
                    Selected = unitTestTypeId == null
                });

                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = string.Empty,
                    Selected = false,
                    Disabled = true
                });
            }

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();

            var unitTestTypes = storage.UnitTestTypes.Filter(null, int.MaxValue);

            if (!userOnly)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = "- Системные -",
                    Selected = false,
                    Disabled = true
                });

                items.AddRange(unitTestTypes
                    .Where(t => t.IsSystem)
                    .OrderBy(t => t.DisplayName)
                    .Select(t => new SelectListItem()
                    {
                        Value = t.Id.ToString(),
                        Text = t.DisplayName,
                        Selected = t.Id == unitTestTypeId
                    }));
            }

            var userUnitTestTypes = unitTestTypes.Where(t => !t.IsSystem);

            if (userUnitTestTypes.Any())
            {

                if (!userOnly)
                {
                    items.Add(new SelectListItem()
                    {
                        Value = string.Empty,
                        Text = string.Empty,
                        Selected = false,
                        Disabled = true
                    });

                    items.Add(new SelectListItem()
                    {
                        Value = string.Empty,
                        Text = "- Пользовательские -",
                        Selected = false,
                        Disabled = true
                    });
                }

                items.AddRange(userUnitTestTypes
                        .OrderBy(t => t.DisplayName)
                        .Select(t => new SelectListItem()
                        {
                            Value = t.Id.ToString(),
                            Text = t.DisplayName,
                            Selected = t.Id == unitTestTypeId
                        }))
                    ;
            }
            return items;
        }

        public static List<SelectListItem> GetLogLevelItems(LogLevel? level)
        {
            var levels = new LogLevel[]
            {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Fatal
            };
            return levels.Select(logLevel => new SelectListItem()
            {
                Text = logLevel.ToString(),
                Value = logLevel.ToString(),
                Selected = logLevel == level
            }).ToList();
        }

        public static List<SelectListItem> GetEventImportanceItems(EventImportance? importance, bool allowEmpty)
        {
            var items = new EventImportance[]
            {
                EventImportance.Alarm,
                EventImportance.Warning,
                EventImportance.Success,
                EventImportance.Unknown
            };
            var result = items.Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.ToString(),
                Selected = t == importance
            }).ToList();
            if (allowEmpty)
                result.Insert(0, new SelectListItem()
                {
                    Text = "Любая",
                    Value = string.Empty,
                    Selected = importance == null
                });
            return result;
        }

        public static List<SelectListItem> GetEventCategoryItems(EventCategory? category, bool allowEmpty)
        {
            var items = new EventCategory[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent,
                EventCategory.ComponentExternalStatus,
                EventCategory.UnitTestResult
            };
            var result = items.Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.ToString(),
                Selected = t == category
            }).ToList();

            if (allowEmpty)
            {
                result.Insert(0, new SelectListItem()
                {
                    Text = "Любая",
                    Value = string.Empty,
                    Selected = category == null
                });
            }

            return result;
        }

        public static MultiSelectList GetComponentStatusItems(MonitoringStatus[] statuses)
        {
            var allStatuses = new MonitoringStatus[]
            {
                MonitoringStatus.Alarm,
                MonitoringStatus.Warning,
                MonitoringStatus.Success,
                MonitoringStatus.Disabled,
                MonitoringStatus.Unknown
            };
            var items = allStatuses.Select(t => new
            {
                Value = t.ToString(),
                Text = t.ToString()
            });
            var selectedItems = statuses.Select(t => t.ToString()).ToArray();
            var result = new MultiSelectList(items, "Value", "Text", selectedItems);
            return result;
        }

        public static List<SelectListItem> GetNotificationStatusItems(NotificationStatus? status, bool allowEmpty)
        {
            var items = new NotificationStatus[]
            {
                NotificationStatus.InQueue,
                NotificationStatus.Processed,
                NotificationStatus.Sent,
                NotificationStatus.Error
            };
            var result = items.Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.ToString(),
                Selected = t == status
            }).ToList();
            if (allowEmpty)
                result.Insert(0, new SelectListItem()
                {
                    Text = "Любой",
                    Value = string.Empty,
                    Selected = status == null
                });
            return result;
        }

        public static List<SelectListItem> GetRolesItems(Guid? selected)
        {
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var roles = storage.Roles.GetAll().OrderBy(t => t.DisplayName);

            var items = roles
                .Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.DisplayName,
                    Selected = x.Id == selected
                })
                .ToList();

            items.Insert(0, new SelectListItem());

            return items;
        }

        public static string GetComponentLogLevels(LogConfigForRead logConfig)
        {
            if (logConfig != null)
            {
                if (!logConfig.Enabled)
                    return "Логирование отключено";
                var list = new List<LogLevel>();
                if (logConfig.IsFatalEnabled)
                    list.Add(LogLevel.Fatal);
                if (logConfig.IsErrorEnabled)
                    list.Add(LogLevel.Error);
                if (logConfig.IsWarningEnabled)
                    list.Add(LogLevel.Warning);
                if (logConfig.IsInfoEnabled)
                    list.Add(LogLevel.Info);
                if (logConfig.IsDebugEnabled)
                    list.Add(LogLevel.Debug);
                if (logConfig.IsTraceEnabled)
                    list.Add(LogLevel.Trace);
                if (list.Count > 0)
                    return "Включены " + string.Join(", ", list);
                else
                    return "Все уровни логирования отключены";
            }
            else
            {
                return "Нет конфига";
            }
        }

        public static List<SelectListItem> GetAccountMetricTypes(Guid? metricTypeId, Guid componentId, bool allowEmpty)
        {
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = string.Empty,
                    Text = string.Empty,
                    Selected = metricTypeId == null
                });
            }

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();

            var metricTypeIds = storage.Metrics.GetByComponentId(componentId).Select(t => t.MetricTypeId).Distinct().ToArray();

            var metricTypes = storage.MetricTypes.GetMany(metricTypeIds).OrderBy(t => t.DisplayName);

            items.AddRange(metricTypes.Select(t => new SelectListItem()
            {
                Value = t.Id.ToString(),
                Text = t.SystemName,
                Selected = t.Id == metricTypeId
            }));
            return items;
        }

        public static List<SelectListItem> GetTimeZoneItems(int offsetMinutes)
        {
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();

            var result = storage.TimeZones.GetAll()
                .OrderBy(t => t.OffsetMinutes)
                .Select(t => new SelectListItem()
                {
                    Value = t.OffsetMinutes.ToString(),
                    Text = t.Name,
                    Selected = t.OffsetMinutes == offsetMinutes
                })
                .ToList();
            return result;
        }

        public static List<SelectListItem> GetEnumItems(Type type, object selectedItem, bool allowEmpty)
        {
            return GetEnumItems(type, selectedItem, allowEmpty, null);
        }

        public static List<SelectListItem> GetEnumItems(Type type, object selectedItem, bool allowEmpty, List<object> allItems)
        {
            var items = new List<SelectListItem>();

            var method = ReflectionHelper.MethodOf(() => EnumHelper.EnumToString(0)).GetGenericMethodDefinition(); // Refactor safe
            var generic = method.MakeGenericMethod(type);
            var enumItems = Enum.GetValues(type);
            if (allItems != null)
            {
                enumItems = allItems.ToArray();
            }

            foreach (var enumItem in enumItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = enumItem.ToString(),
                    Text = (string)generic.Invoke(null, new[] { enumItem }),
                    Selected = enumItem.Equals(selectedItem)
                });
            }

            if (allowEmpty)
            {
                items.Insert(0, new SelectListItem()
                {
                    Value = string.Empty,
                    Text = string.Empty,
                    Selected = selectedItem == null
                });
            }

            return items;
        }

        public static List<SelectListItem> GetUserContactTypeItems(UserContactType? contactType, bool allowEmpty)
        {
            var items = new[]
            {
                UserContactType.Email,
                UserContactType.MobilePhone,
                UserContactType.Telegram,
                UserContactType.VKontakte
            };
            var result = items.Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.ToString(),
                Selected = t == contactType
            }).ToList();
            if (allowEmpty)
                result.Insert(0, new SelectListItem()
                {
                    Text = "Любой",
                    Value = string.Empty,
                    Selected = contactType == null
                });
            return result;
        }

        public static void SetTempMessage(this Controller controller, TempMessageType type, string message)
        {
            var tempmessage = new TempMessage()
            {
                Type = type,
                Message = message
            };
            controller.TempData["Alert"] = JsonConvert.SerializeObject(tempmessage);
        }

        public static TempMessage GetTempMessage(HttpContext httpContext)
        {
            var currentContext = FullRequestContext.GetCurrent(httpContext);
            if (currentContext == null)
                return null;
            var json = (string)currentContext.Controller.TempData["Alert"];
            return json != null ? JsonConvert.DeserializeObject<TempMessage>(json) : null;
        }

        public static string FormatSize(Int64 value)
        {
            if (value > 1024 * 1024 * 1024)
                return value / (1024 * 1024 * 1024) + " GB";

            if (value > 1024 * 1024)
                return value / (1024 * 1024) + " MB";

            if (value > 1024)
                return value / 1024 + " KB";

            return value + " B";
        }

        public static string TimeSpanAsString(TimeSpan? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var list = TimeSpanAsList(value);

            return string.Join(" ", list);
        }

        public static string TimeSpanAs2UnitString(TimeSpan? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var list = TimeSpanAsList(value);

            list = list.Take(2).ToList();

            return string.Join(" ", list);
        }

        public static List<string> TimeSpanAsList(TimeSpan? value)
        {
            var list = new List<string>();

            if (!value.HasValue)
                return list;

            var days = (int)value.Value.TotalDays;
            var hours = value.Value.Hours;
            var minutes = value.Value.Minutes;
            var seconds = value.Value.Seconds;

            // дни
            if (days > 0)
            {
                string daysUnit;
                if (days % 10 == 1)
                    daysUnit = "день";
                else if (days % 10 <= 4)
                    daysUnit = "дня";
                else
                    daysUnit = "дней";

                list.Add(days + " " + daysUnit);
            }

            // часы
            if (hours > 0)
            {
                list.Add(hours + " час");
            }

            // минуты
            if (minutes > 0)
            {
                list.Add(minutes + " мин");
            }

            // секунды
            if (seconds > 0)
            {
                list.Add(seconds + " сек");
            }

            if (list.Count == 0)
                list.Add("0 сек");

            return list;
        }

        public static string TimeSpanAsShortString(TimeSpan? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var days = (int)value.Value.TotalDays;
            var hours = value.Value.Hours;
            var minutes = value.Value.Minutes;
            var seconds = value.Value.Seconds;

            var list = new List<string>();
            if (days > 0)
                list.Add(days + "д");
            if (hours > 0)
                list.Add(hours + "ч");
            if (minutes > 0)
                list.Add(minutes + "м");
            if (seconds > 0)
                list.Add(seconds + "с");

            return string.Join(" ", list);
        }

        public static string GetUrlDateTimeString(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            return DateTimeHelper.ToUrlFormat(date.Value);
        }

        public const string GridMvcDateTimeDisplayFormat = "{0:dd.MM.yyyy HH:mm:ss}";

        public static string DateTimeToHighChartsFormat(DateTime date)
        {
            return "Date.UTC(" + date.Year + ", " + (date.Month - 1) + ", " + date.Day + ", " + date.Hour + ", " + date.Minute + ", " + date.Second + ")";
        }

        public static string DateToJs(DateTime date)
        {
            return "new Date(" + date.Year + ", " + (date.Month - 1) + ", " + date.Day + ", " + date.Hour + ", " + date.Minute + ").valueOf()";
        }

        // Д. закомментировал, так как непонятно, для чего и где используется этот метод
        /*
        public static IHtmlContent AntiForgeryTokenForAjaxPost(this IHtmlHelper helper)
        {
            var antiForgeryInputTag = helper.AntiForgeryToken().ToString();
            // Above gets the following: <input name="__RequestVerificationToken" type="hidden" value="PnQE7R0MIBBAzC7SqtVvwrJpGbRvPgzWHo5dSyoSaZoabRjf9pCyzjujYBU_qKDJmwIOiPRDwBV1TNVdXFVgzAvN9_l2yt9-nf4Owif0qIDz7WRAmydVPIm6_pmJAI--wvvFQO7g0VvoFArFtAR2v6Ch1wmXCZ89v0-lNOGZLZc1" />
            var removedStart = antiForgeryInputTag.Replace(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""", "");
            var tokenValue = removedStart.Replace(@""" />", "");
            if (antiForgeryInputTag == removedStart || removedStart == tokenValue)
                throw new InvalidOperationException("Oops! The Html.AntiForgeryToken() method seems to return something I did not expect.");
            return new IHtmlContent(string.Format(@"{0}:""{1}""", "__RequestVerificationToken", tokenValue));
        }
        */

        public static int GetDecimalPlaces(decimal value)
        {
            value = Math.Abs(value);
            value -= (int)value;
            var decimalPlaces = 0;
            while (value > 0)
            {
                decimalPlaces++;
                value *= 10;
                value -= (int)value;
            }
            return decimalPlaces;
        }

        public static string HtmlBlock(this IHtmlHelper helper, Func<object, IHtmlContent> block)
        {
            var content = block.Invoke(null);
            using var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

    }

}