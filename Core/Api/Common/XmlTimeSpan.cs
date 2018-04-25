using System;
using System.Xml;
using System.Xml.Serialization;

namespace Zidium.Core.Api
{
    public class XmlTimeSpan
    {
        private TimeSpan? _value = null;

        public XmlTimeSpan()
            : this(TimeSpan.Zero)
        {
        }

        public XmlTimeSpan(TimeSpan input)
        {
            _value = input;
        }

        public static implicit operator TimeSpan(XmlTimeSpan input)
        {
            if (input == null || input._value == null)
            {
                throw new Exception("XmlTimeSpan is NULL");
            }
            return input.ToTimeSpan();
        }

        public TimeSpan ToTimeSpan()
        {
            if (_value == null)
            {
                throw new Exception("XmlTimeSpan is NULL");
            }
            return _value.Value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan input)
        {
            return new XmlTimeSpan(input);
        }

        public void FromTimeSpan(TimeSpan input)
        {
            _value = input;
        }

        [XmlText]
        public string Value
        {
            get
            {
                if (_value == null)
                {
                    return null;
                }
                long seconds = (long)_value.Value.TotalSeconds;
                return XmlConvert.ToString(seconds);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                long seconds = XmlConvert.ToInt64(value);
                _value = TimeSpan.FromSeconds(seconds);
            }
        }
    }
}
