using System;
using System.Linq;
using Zidium.Storage;

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

            public Api.Dto.DataType DataType { get; set; }

            public string Name { get; set; }

            public string Value { get; set; }
        }

        public Row[] Rows { get; set; }

        public bool ShowDataTypes { get; set; }

        public bool ShowHeaders { get; set; }

        public ExtentionPropertyOwner Owner { get; private set; }

        public static ExtentionPropertiesModel Create(EventPropertyForRead[] properties)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Event
            };
            model.Rows = properties.Select(property => new Row()
            {
                Id = property.Id,
                DataType = property.DataType,
                Name = property.Name,
                Value = property.Value
            }).ToArray();
            return model;
        }

        public static ExtentionPropertiesModel Create(ComponentPropertyForRead[] properties)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Component
            };
            model.Rows = properties.Select(property => new Row()
            {
                Id = property.Id,
                DataType = property.DataType,
                Name = property.Name,
                Value = property.Value
            }).ToArray();
            return model;
        }

        public static ExtentionPropertiesModel Create(LogPropertyForRead[] properties)
        {
            var model = new ExtentionPropertiesModel()
            {
                Owner = ExtentionPropertyOwner.Log,
                ShowHeaders = false
            };
            model.Rows = properties.Select(property => new Row()
            {
                Id = property.Id,
                DataType = property.DataType,
                Name = property.Name,
                Value = property.Value
            }).ToArray();
            return model;
        }
    }
}