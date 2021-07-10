using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models.CheckModels;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.HttpRequestCheckModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        public override string ComponentLabelText
        {
            get { return "Выберите компонент (хост)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка сайта"; }
        }

        public override UserFolder NewComponentFolder
        {
            get { return UserFolder.WebSites; }
        }

        public override Guid NewComponentTypeId
        {
            get { return SystemComponentType.WebSite.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.HttpUnitTestType.Id; }
        }

        [Display(Name = "Url запроса")]
        public string Url { get; set; }

        [Display(Name = "Метод")]
        public HttpRequestMethod Method { get; set; }

        [Display(Name = "Тело запроса (только для POST)")]
        [MaxLength(4000)]
        public string Body { get; set; }

        [Display(Name = "Код ответа")]
        public int ResponseCode { get; set; }

        [Display(Name = "Ожидаемый фрагмент Html")]
        public string SuccessHtml { get; set; }

        [Display(Name = "Недопустимый фрагмент Html")]
        public string ErrorHtml { get; set; }

        [Display(Name = "Таймаут, сек")]
        public int TimeOutSeconds { get; set; }

        public List<KeyValueRowModel> RequestCookies { get; set; }

        public List<KeyValueRowModel> RequestHeaders { get; set; }

        public List<KeyValueRowModel> WebFormDatas { get; set; }

        public EditModel()
        {
            RequestHeaders = new List<KeyValueRowModel>();
            WebFormDatas = new List<KeyValueRowModel>();
            RequestCookies = new List<KeyValueRowModel>();
        }

        public List<SelectListItem> GetMethodItems()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = "POST",
                Value = "Post",
            });
            items.Add(new SelectListItem()
            {
                Text = "GET",
                Value = "Get",
            });
            foreach (var item in items)
            {
                item.Selected = string.Equals(
                    item.Value,
                    Method.ToString(),
                    StringComparison.InvariantCultureIgnoreCase);
            }
            return items;
        }

        protected override void ValidateRule()
        {
            if (TimeOutSeconds < 1)
            {
                throw new UserFriendlyException("Таймаут выполнения должен быть больше нуля");
            }

            Uri uri;
            try
            {
                uri = new Uri(Url);
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Неверный формат URL");
            }

            var parts = uri.Host.Split('.');
            if (parts.Length <= 1)
                throw new UserFriendlyException("Неверный формат домена");
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            HttpRequestUnitTestRuleForRead rule = null;
            if (id != null)
            {
                rule = storage.HttpRequestUnitTestRules.GetByUnitTestId(id.Value).FirstOrDefault();
            }
            else
            {
                Period = TimeSpan.FromMinutes(10);
            }

            if (rule != null)
            {
                Method = rule.Method;
                Body = rule.Body;
                TimeOutSeconds = rule.TimeoutSeconds ?? 10;
                Url = rule.Url;
                SuccessHtml = rule.SuccessHtml;
                ErrorHtml = rule.ErrorHtml;
                ResponseCode = rule.ResponseCode ?? 200;
                RequestHeaders = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestHeader, storage);
                WebFormDatas = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.WebFormData, storage);
                RequestCookies = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestCookie, storage);
            }
            else
            {
                ResponseCode = 200;
                TimeOutSeconds = 5;
                Method = HttpRequestMethod.Get;
            }
        }

        public void SaveRule(Guid id, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var httpRequestUnitTest = storage.HttpRequestUnitTests.GetOneOrNullByUnitTestId(id);
                if (httpRequestUnitTest == null)
                {
                    var httpRequestUnitTestForAdd = new HttpRequestUnitTestForAdd()
                    {
                        UnitTestId = id
                    };
                    storage.HttpRequestUnitTests.Add(httpRequestUnitTestForAdd);
                }

                var rule = storage.HttpRequestUnitTestRules.GetByUnitTestId(id).FirstOrDefault();
                if (rule == null)
                {
                    var ruleForAdd = new HttpRequestUnitTestRuleForAdd()
                    {
                        Id = Guid.NewGuid(),
                        HttpRequestUnitTestId = id,
                        DisplayName = CheckName,
                        Url = new Uri(Url).AbsoluteUri
                    };
                    storage.HttpRequestUnitTestRules.Add(ruleForAdd);
                    rule = storage.HttpRequestUnitTestRules.GetOneById(ruleForAdd.Id);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.DisplayName.Set(CheckName);
                ruleForUpdate.SortNumber.Set(0);
                ruleForUpdate.TimeoutSeconds.Set(TimeOutSeconds);
                ruleForUpdate.Method.Set(Method);
                ruleForUpdate.Body.Set(Body);
                ruleForUpdate.Url.Set(new Uri(Url).AbsoluteUri);
                ruleForUpdate.ResponseCode.Set(ResponseCode);
                ruleForUpdate.SuccessHtml.Set(SuccessHtml);
                ruleForUpdate.ErrorHtml.Set(ErrorHtml);
                storage.HttpRequestUnitTestRules.Update(ruleForUpdate);

                var ruleDatas = storage.HttpRequestUnitTestRuleDatas.GetByRuleId(rule.Id);
                foreach (var data in ruleDatas)
                {
                    storage.HttpRequestUnitTestRuleDatas.Delete(data.Id);
                }

                if (RequestHeaders != null)
                    RequestHeaders.ForEach(x =>
                        AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.RequestHeader, storage));

                if (RequestCookies != null)
                    RequestCookies.ForEach(x =>
                        AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.RequestCookie, storage));

                if (WebFormDatas != null)
                    WebFormDatas.ForEach(
                        x => AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.WebFormData, storage));

                transaction.Commit();
            }
        }

        protected List<KeyValueRowModel> GetRuleDatas(HttpRequestUnitTestRuleForRead rule, HttpRequestUnitTestRuleDataType type, IStorage storage)
        {
            var ruleDatas = storage.HttpRequestUnitTestRuleDatas.GetByRuleId(rule.Id);
            var datas = ruleDatas.Where(x => x.Type == type).ToList();
            var rows = new List<KeyValueRowModel>();
            foreach (var data in datas)
            {
                var row = ConvertToKeyValueRow(data);
                rows.Add(row);
            }
            return rows;
        }

        protected KeyValueRowModel ConvertToKeyValueRow(HttpRequestUnitTestRuleDataForRead data)
        {
            return new KeyValueRowModel()
            {
                Id = data.Id.ToString(),
                Key = data.Key,
                Value = data.Value
            };
        }

        protected void AddRuleData(HttpRequestUnitTestRuleForRead rule, KeyValueRowModel row, HttpRequestUnitTestRuleDataType type, IStorage storage)
        {
            var data = new HttpRequestUnitTestRuleDataForAdd()
            {
                Id = Guid.NewGuid(),
                Key = row.Key,
                Value = row.Value,
                Type = type,
                RuleId = rule.Id
            };
            storage.HttpRequestUnitTestRuleDatas.Add(data);
        }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

    }
}