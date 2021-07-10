using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models.HttpRequestCheckModels
{
    public class HttpRequestEditModel
    {
        public Guid CheckId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string PostData { get; set; }

        public string Method { get; set; }

        public ICollection<KeyValuePair<string, string>> Properties { get; set; }

        public List<SelectListItem> GetMethodItems()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Value = "GET"
            });
            items.Add(new SelectListItem()
            {
                Value = "POST"
            });
            foreach (var item in items)
            {
                item.Text = item.Value;
                item.Selected = item.Value == Method;
            }
            return items;
        } 
    }
}