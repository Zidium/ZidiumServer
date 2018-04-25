using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.ExtentionProperties
{
    public class ExtentionPropertiesModel
    {
        /// <summary>
        /// Модель расширенных свойств (событий, компонентов и т.д.)
        /// </summary>
        public class Row
        {
            public Guid Id { get; set; }

            public DataType DataType { get; set; }

            public string Name { get; set; }

            public string Value { get; set; }
        }

        public List<Row> Rows { get; set; }

        public bool ShowDataTypes { get; set; }

        public bool ShowHeaders { get; set; }

        public ExtentionPropertyOwner Owner { get; private set; }

        public static ExtentionPropertiesModel Create(Event eventObj)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Event
            };
            model.Rows = new List<Row>();
            foreach (var eventProperty in eventObj.Properties)
            {
                var row = new Row()
                {
                    Id = eventProperty.Id,
                    DataType = eventProperty.DataType,
                    Name = eventProperty.Name,
                    Value = eventProperty.Value
                };
                model.Rows.Add(row);
            }
            return model;
        }

        public static ExtentionPropertiesModel Create(Component component)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Component
            };
            model.Rows = new List<Row>();
            foreach (var property in component.Properties)
            {
                var row = new Row()
                {
                    Id = property.Id,
                    DataType = property.DataType,
                    Name = property.Name,
                    Value = property.Value
                };
                model.Rows.Add(row);
            }
            return model;
        }

        public static ExtentionPropertiesModel Create(Log log)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Log,
                ShowHeaders = false
            };
            model.Rows = new List<Row>();
            foreach (var property in log.Parameters)
            {
                var row = new Row()
                {
                    Id = property.Id,
                    DataType = property.DataType,
                    Name = property.Name,
                    Value = property.Value
                };
                model.Rows.Add(row);
            }
            return model;
        }
    }
}