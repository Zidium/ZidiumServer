using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests;
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
            get { return SystemComponentTypes.WebSite.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestTypes.HttpUnitTestType.Id; }
        }

        [Display(Name = "Url запроса")]
        public string Url { get; set; }

        [Display(Name = "Метод")]
        public HttpRequestMethod Method { get; set; }

        [Display(Name = "Код ответа")]
        public int ResponseCode { get; set; }

        [AllowHtml]
        [Display(Name = "Ожидаемый фрагмент Html")]
        public string SuccessHtml { get; set; }

        [AllowHtml]
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

            Uri uri = null;
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

        public void LoadRule()
        {
            HttpRequestUnitTestRule rule = null;
            if (UnitTest != null)
            {
                rule = UnitTest.HttpRequestUnitTest.Rules.FirstOrDefault();
            }
            else
            {
                Period = TimeSpan.FromMinutes(10);
            }

            if (rule != null)
            {
                Method = rule.Method;
                TimeOutSeconds = rule.TimeoutSeconds ?? 10;
                Url = rule.Url;
                SuccessHtml = rule.SuccessHtml;
                ErrorHtml = rule.ErrorHtml;
                ResponseCode = rule.ResponseCode ?? 200;
                RequestHeaders = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestHeader);
                WebFormDatas = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.WebFormData);
                RequestCookies = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestCookie);
            }
            else
            {
                ResponseCode = 200;
                TimeOutSeconds = 5;
                Method = HttpRequestMethod.Get;
            }
        }

        public void SaveRule()
        {
            if (UnitTest.HttpRequestUnitTest == null)
            {
                var newHttpRequestUnitTest = new HttpRequestUnitTest
                {
                    UnitTestId = UnitTest.Id,
                    UnitTest = UnitTest
                };
                UnitTest.HttpRequestUnitTest = newHttpRequestUnitTest;
            }

            var rule = UnitTest.HttpRequestUnitTest.Rules.FirstOrDefault();
            if (rule == null)
            {
                rule = new HttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    HttpRequestUnitTest = UnitTest.HttpRequestUnitTest
                };
                UnitTest.HttpRequestUnitTest.Rules.Add(rule);
            }

            rule.DisplayName = UnitTest.DisplayName;
            rule.SortNumber = 0;
            rule.TimeoutSeconds = TimeOutSeconds;
            rule.Method = Method;
            rule.Url = new Uri(Url).AbsoluteUri;
            rule.ResponseCode = ResponseCode;
            rule.SuccessHtml = SuccessHtml;
            rule.ErrorHtml = ErrorHtml;

            foreach (var data in rule.Datas.ToArray())
            {
                rule.Datas.Remove(data);
                AccountDbContext.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            }

            if (RequestHeaders != null)
                RequestHeaders.ForEach(x => AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.RequestHeader));

            if (RequestCookies != null)
                RequestCookies.ForEach(x => AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.RequestCookie));

            if (WebFormDatas != null)
                WebFormDatas.ForEach(x => AddRuleData(rule, x, HttpRequestUnitTestRuleDataType.WebFormData));
        }

        protected List<KeyValueRowModel> GetRuleDatas(HttpRequestUnitTestRule rule, HttpRequestUnitTestRuleDataType type)
        {
            var datas = rule.Datas.Where(x => x.Type == type).ToList();
            var rows = new List<KeyValueRowModel>();
            foreach (var data in datas)
            {
                var row = ConvertToKeyValueRow(data);
                rows.Add(row);
            }
            return rows;
        }

        protected KeyValueRowModel ConvertToKeyValueRow(HttpRequestUnitTestRuleData data)
        {
            return new KeyValueRowModel()
            {
                Id = data.Id.ToString(),
                Key = data.Key,
                Value = data.Value
            };
        }

        protected void AddRuleData(HttpRequestUnitTestRule rule, KeyValueRowModel row, HttpRequestUnitTestRuleDataType type)
        {
            var data = new HttpRequestUnitTestRuleData
            {
                Id = Guid.NewGuid(),
                Key = row.Key,
                Value = row.Value,
                Type = type,
                Rule = rule,
                RuleId = rule.Id
            };
            rule.Datas.Add(data);
        }
    }
}