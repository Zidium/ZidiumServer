using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ExtentionPropertyValue
    {
        public object Value { get; set; }

        public DataType DataType { get; set; }

        public bool HasValue()
        {
            return Value != null;
        }

        #region String

        public static implicit operator string(ExtentionPropertyValue value)
        {
            if (value == null || value.Value == null)
            {
                return null;
            }
            return value.Value.ToString();//todo могуть быть проблемы с локализацией дат и дробных чисел
        }

        public static implicit operator ExtentionPropertyValue(string value)
        {
            return new ExtentionPropertyValue()
            {
                DataType = DataType.String,
                Value = value
            };
        }

        #endregion

        #region Int32

        public static implicit operator Int32(ExtentionPropertyValue value)
        {
            return (int)value.Value;
        }

        public static implicit operator Int32?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as Int32?;
        }

        public static implicit operator ExtentionPropertyValue(Int32 value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Int32
            };
        }

        #endregion

        #region Boolean

        public static implicit operator Boolean(ExtentionPropertyValue value)
        {
            return (bool)value.Value;
        }

        public static implicit operator Boolean?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as bool?;
        }

        public static implicit operator ExtentionPropertyValue(Boolean value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Boolean
            };
        }

        #endregion

        #region Int64

        public static implicit operator Int64(ExtentionPropertyValue value)
        {
            return (Int64)value.Value;
        }

        public static implicit operator Int64?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as Int64?;
        }

        public static implicit operator ExtentionPropertyValue(Int64 value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Int64
            };
        }

        #endregion

        #region Double

        public static implicit operator Double(ExtentionPropertyValue value)
        {
            return (double)value.Value;
        }

        public static implicit operator Double?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as double?;
        }

        public static implicit operator ExtentionPropertyValue(Double value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Double
            };
        }

        #endregion

        #region binary

        public static implicit operator byte[](ExtentionPropertyValue value)
        {
            if (value == null || value.Value==null)
            {
                return null;
            }
            return value.Value as byte[];
        }

        public static implicit operator ExtentionPropertyValue(byte[] value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Binary
            };
        }

        #endregion

        #region DateTime

        public static implicit operator DateTime(ExtentionPropertyValue value)
        {
            return (DateTime)value.Value;
        }

        public static implicit operator DateTime?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as DateTime?;
        }

        public static implicit operator ExtentionPropertyValue(DateTime value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.DateTime
            };
        }

        #endregion

        #region Guid

        public static implicit operator Guid(ExtentionPropertyValue value)
        {
            return (Guid)value.Value;
        }

        public static implicit operator Guid?(ExtentionPropertyValue value)
        {
            if (value == null)
            {
                return null;
            }
            return value.Value as Guid?;
        }

        public static implicit operator ExtentionPropertyValue(Guid value)
        {
            return new ExtentionPropertyValue()
            {
                Value = value,
                DataType = DataType.Guid
            };
        }

        #endregion

        public override string ToString()
        {
            string text = this;
            return text;
        }
    }
}
