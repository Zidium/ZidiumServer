using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;

namespace Zidium.Api.Common
{
    // TODO split class
    public static class DataConverter
    {
        public static string GetXmlValue(ExtentionPropertyValue value)
        {
            if (value == null || value.Value == null)
            {
                return null;
            }
            if (value.DataType == DataType.Binary)
            {
                return Convert.ToBase64String((byte[])value.Value);
            }
            if (value.DataType == DataType.Boolean)
            {
                return Convert.ToString((bool)value.Value);
            }
            if (value.DataType == DataType.DateTime)
            {
                return Convert.ToString((DateTime)value.Value);
            }
            if (value.DataType == DataType.Double)
            {
                return Convert.ToString((Double)value.Value);
            }
            if (value.DataType == DataType.Int32)
            {
                return Convert.ToString((Int32)value.Value);
            }
            if (value.DataType == DataType.Int64)
            {
                return Convert.ToString((Int64)value.Value);
            }
            if (value.DataType == DataType.String)
            {
                return (string)value.Value;
            }
            if (value.DataType == DataType.Guid)
            {
                return Convert.ToString((Guid)value.Value);
            }
            if (value.DataType == DataType.Unknown)
            {
                return (string)value.Value;
            }
            throw new Exception("Неизвестное значение DataType: " + value.DataType);
        }

        public static ExtentionPropertyCollection GetExtentionPropertyCollection(List<ExtentionPropertyDto> propertyDtos)
        {
            if (propertyDtos == null)
            {
                return new ExtentionPropertyCollection();
            }
            var collection = new ExtentionPropertyCollection();
            foreach (var propertyDto in propertyDtos)
            {
                if (propertyDto != null && propertyDto.Name != null)
                {
                    var property = GetExtentionPropertyFromDto(propertyDto);
                    if (property != null)
                    {
                        collection.Add(property);
                    }
                }
            }
            return collection;
        }

        public static ExtentionProperty GetExtentionPropertyFromDto(ExtentionPropertyDto propertyDto)
        {
            if (propertyDto == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(propertyDto.Name))
            {
                return null;
            }
            var typeEnum = propertyDto.Type;
            object valueObj = null;
            if (string.IsNullOrEmpty(propertyDto.Value) == false)
            {
                if (typeEnum == DataType.Binary)
                {
                    valueObj = Convert.FromBase64String(propertyDto.Value);
                }
                else if (typeEnum == DataType.Boolean)
                {
                    valueObj = Convert.ToBoolean(propertyDto.Value);
                }
                else if (typeEnum == DataType.DateTime)
                {
                    valueObj = Convert.ToDateTime(propertyDto.Value);
                }
                else if (typeEnum == DataType.Double)
                {
                    valueObj = Convert.ToDouble(propertyDto.Value);
                }
                else if (typeEnum == DataType.Int32)
                {
                    valueObj = Convert.ToInt32(propertyDto.Value);
                }
                else if (typeEnum == DataType.Int64)
                {
                    valueObj = Convert.ToInt64(propertyDto.Value);
                }
                else if (typeEnum == DataType.String)
                {
                    valueObj = propertyDto.Value;
                }
                else if (typeEnum == DataType.Guid)
                {
                    valueObj = Guid.Parse(propertyDto.Value);
                }
                else if (typeEnum == DataType.Unknown)
                {
                    valueObj = propertyDto.Value;
                }
                else
                {
                    throw new Exception("Неизвестное значение DataType: " + typeEnum);
                }
            }
            return new ExtentionProperty(propertyDto.Name)
            {
                Value = new ExtentionPropertyValue()
                {
                    Value = valueObj,
                    DataType = typeEnum
                }
            };
        }

        public static List<ExtentionPropertyDto> GetExtentionPropertyDtos(ExtentionPropertyCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            return collection.Select(x => new ExtentionPropertyDto()
            {
                Name = x.Name,
                Value = GetXmlValue(x.Value),
                Type = x.Value.DataType
            }).ToList();
        }

        public static TimeSpan? GetTimeSpanFromSeconds(double? seconds)
        {
            if (seconds == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(seconds.Value);
        }

        public static double? GetSeconds(TimeSpan? timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            return Math.Round(timeSpan.Value.TotalSeconds);
        }

    }
}
