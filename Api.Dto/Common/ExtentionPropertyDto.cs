﻿using System;
using System.Globalization;

namespace Zidium.Api.Dto
{
    public class ExtentionPropertyDto
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DataType Type { get; set; }

        // TODO move to extension
        public long GetSize()
        {
            long size = sizeof(byte); // Type
            if (Name != null)
            {
                size += Name.Length * sizeof(char);
            }
            if (Value != null)
            {
                size += Value.Length * sizeof(char);
            }
            return size;
        }

        public static ExtentionPropertyDto Create(string key, string value)
        {
            return new ExtentionPropertyDto()
            {
                Name = key,
                Value = value,
                Type = DataType.String
            };
        }

        public static ExtentionPropertyDto Create(string key, int value)
        {
            return new ExtentionPropertyDto()
            {
                Name = key,
                Value = value.ToString(),
                Type = DataType.Int32
            };
        }

        public static ExtentionPropertyDto Create(string key, double value)
        {
            return new ExtentionPropertyDto()
            {
                Name = key,
                Value = value.ToString(CultureInfo.InvariantCulture),
                Type = DataType.Double
            };
        }

        public static ExtentionPropertyDto Create(string key, Guid value)
        {
            return new ExtentionPropertyDto()
            {
                Name = key,
                Value = value.ToString(),
                Type = DataType.Guid
            };
        }
    }
}
