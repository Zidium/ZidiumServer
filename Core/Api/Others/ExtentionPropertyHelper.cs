using System;
using System.Collections.Generic;

namespace Zidium.Core.Api
{
    public static class ExtentionPropertyHelper
    {
        public static long GetSize(this List<ExtentionPropertyDto> properties)
        {
            long size = 0;
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (property != null)
                    {
                        size += property.GetSize();
                    }
                }
            }
            return size;
        }

        public static void AddValue(this List<ExtentionPropertyDto> properties, string key, string value)
        {
            var property = ExtentionPropertyDto.Create(key, value);
            properties.Add(property);
        }

        public static void AddValue(this List<ExtentionPropertyDto> properties, string key, int value)
        {
            var property = ExtentionPropertyDto.Create(key, value);
            properties.Add(property);
        }

        public static void AddValue(this List<ExtentionPropertyDto> properties, string key, double value)
        {
            var property = ExtentionPropertyDto.Create(key, value);
            properties.Add(property);
        }

        public static void AddValue(this List<ExtentionPropertyDto> properties, string key, Guid value)
        {
            var property = ExtentionPropertyDto.Create(key, value);
            properties.Add(property);
        }

        public static string GetValue(this List<ExtentionPropertyDto> properties, string key)
        {
            if (properties == null)
            {
                return null;
            }
            foreach (var property in properties)
            {
                if (property != null && property.Name == key)
                {
                    return property.Value;
                }
            }
            return null;
        }
    }
}
