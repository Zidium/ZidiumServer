using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsHttpModel
    {
        public Guid Id { get; set; }

        public TimeSpan? Timeout { get; set; }

        public HttpRequestMethod Method { get; set; }

        public string Url { get; set; }

        public string Body { get; set; }

        public List<KeyValueRowModel> RequestCookies { get; set; }

        public List<KeyValueRowModel> RequestHeaders { get; set; }

        public List<KeyValueRowModel> WebFormDatas { get; set; }

        public int? ResponseCode { get; set; }

        public string SuccessFragment { get; set; }

        public string ErrorFragment { get; set; }

        private static KeyValueRowModel ConvertToKeyValueRow(HttpRequestUnitTestRuleData data)
        {
            return new KeyValueRowModel()
            {
                Id = data.Id.ToString(),
                Key = data.Key,
                Value = data.Value
            };
        }

        private static List<KeyValueRowModel> GetRuleDatas(HttpRequestUnitTestRule rule, HttpRequestUnitTestRuleDataType type)
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

        public static ShowSettingsHttpModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }          

            var rule = unitTest.HttpRequestUnitTest.Rules.First();

            var model = new ShowSettingsHttpModel()
            {
                Id = unitTest.Id,
                Body = rule.Body,
                Method = rule.Method,
                Timeout = TimeSpanHelper.FromSeconds(rule.TimeoutSeconds),
                Url = rule.Url,
                ResponseCode = rule.ResponseCode,
                SuccessFragment = rule.SuccessHtml,
                ErrorFragment = rule.ErrorHtml
            };
            model.RequestHeaders = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestHeader);
            model.WebFormDatas = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.WebFormData);
            model.RequestCookies = GetRuleDatas(rule, HttpRequestUnitTestRuleDataType.RequestCookie);
           
            return model;
        }
    }
}