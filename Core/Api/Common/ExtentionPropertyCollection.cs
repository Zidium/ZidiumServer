//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml;
//using System.Xml.Schema;
//using System.Xml.Serialization;

//namespace Zidium.Core.Api
//{
//    /// <summary>
//    /// Коллекция расширенных свойств объекта (события например)
//    /// </summary>
//    [Serializable]
//    public class ExtentionPropertyCollection : ICollection<ExtentionProperty>, IXmlSerializable
//    {
//        protected Dictionary<string, ExtentionProperty> Properties = new Dictionary<string, ExtentionProperty>();

//        /// <summary>
//        /// Записывает и читает значение по ключю. Не кидает исключений (если нет ключа).
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public ExtentionPropertyValue this[string key]
//        {
//            get
//            {
//                ExtentionProperty result = null;
//                Properties.TryGetValue(key, out result);
//                if (result == null)
//                {
//                    return null;
//                }
//                return result.Value;
//            }
//            set
//            {
//                if (Properties.ContainsKey(key))
//                {
//                    Properties[key].Value = value;
//                }
//                else
//                {
//                    var property = new ExtentionProperty()
//                    {
//                        Name = key,
//                        Value = value
//                    };
//                    Properties.Add(key, property);
//                }
//            }
//        }

//        #region Object

//        public void Set(string name, object value)
//        {
//            if (value == null)
//            {
//                this[name] = null as string;//todo
//                return;
//            }
//            var type = value.GetType();
//            if (type == typeof(string))
//            {
//                this[name] = (string)value;
//            }
//            else if (type == typeof(byte))
//            {
//                this[name] = (int)value;
//            }
//            else if (type == typeof(int))
//            {
//                this[name] = (int)value;
//            }
//            else if (type == typeof(double))
//            {
//                this[name] = (double)value;
//            }
//            else if (type == typeof(float))
//            {
//                this[name] = (float)value;
//            }
//            else if (type == typeof(DateTime))
//            {
//                this[name] = (DateTime)value;
//            }
//            else if (type == typeof(long))
//            {
//                this[name] = (long)value;
//            }
//            else if (type == typeof(byte[]))
//            {
//                this[name] = (byte[])value;
//            }
//            else if (type == typeof(bool))
//            {
//                this[name] = (bool)value;
//            }
//            else
//            {
//                this[name] = value.ToString();
//            }
//        }

//        public object GetObject(string name)
//        {
//            var value = this[name];
//            if (value == null)
//            {
//                return null;
//            }
//            if (value.DataType == DataType.String)
//            {
//                return GetStringOrNull(name);
//            }
//            if (value.DataType == DataType.Binary)
//            {
//                return GetStringOrNull(name);
//            }
//            if (value.DataType == DataType.Boolean)
//            {
//                return GetBooleanOrNull(name);
//            }
//            if (value.DataType == DataType.DateTime)
//            {
//                return GetDateTimeOrNull(name);
//            }
//            if (value.DataType == DataType.Double)
//            {
//                return GetDoubleOrNull(name);
//            }
//            if (value.DataType == DataType.Int32)
//            {
//                return GetInt32OrNull(name);
//            }
//            if (value.DataType == DataType.Int64)
//            {
//                return GetInt64OrNull(name);
//            }
//            return GetStringOrNull(name);
//        }

//        #endregion

//        #region Boolean

//        public void Set(string name, bool value)
//        {
//            this[name] = value;
//        }

//        public bool? GetBooleanOrNull(string name)
//        {
//            return this[name];
//        }

//        public bool GetBoolean(string name)
//        {
//            bool? result = GetBooleanOrNull(name);
//            if (result == null)
//            {
//                throw new KeyNotFoundException();
//            }
//            return result.Value;
//        }

//        #endregion

//        #region Int32

//        public void Set(string name, int value)
//        {
//            this[name] = value;
//        }

//        public int? GetInt32OrNull(string name)
//        {
//            return this[name];
//        }

//        public int GetInt32(string name)
//        {
//            int? result = GetInt32OrNull(name);
//            if (result == null)
//            {
//                throw new KeyNotFoundException();
//            }
//            return result.Value;
//        }

//        #endregion

//        #region Int64

//        public void Set(string name, long value)
//        {
//            this[name] = value;
//        }

//        public long? GetInt64OrNull(string name)
//        {
//            return this[name];
//        }

//        public long GetInt64(string name)
//        {
//            long? result = GetInt64OrNull(name);
//            if (result == null)
//            {
//                throw new KeyNotFoundException();
//            }
//            return result.Value;
//        }

//        #endregion

//        #region Double

//        public void Set(string name, double value)
//        {
//            this[name] = value;
//        }

//        public double? GetDoubleOrNull(string name)
//        {
//            return this[name];
//        }

//        public double GetDouble(string name)
//        {
//            double? result = GetDoubleOrNull(name);
//            if (result == null)
//            {
//                throw new KeyNotFoundException();
//            }
//            return result.Value;
//        }

//        #endregion

//        #region DateTime

//        public void Set(string name, DateTime value)
//        {
//            this[name] = value;
//        }

//        public DateTime? GetDateTimeOrNull(string name)
//        {
//            return this[name];
//        }

//        public DateTime GetDateTime(string name)
//        {
//            DateTime? result = GetDateTimeOrNull(name);
//            if (result == null)
//            {
//                throw new KeyNotFoundException();
//            }
//            return result.Value;
//        }

//        #endregion

//        #region String

//        public void Set(string name, string value)
//        {
//            this[name] = value;
//        }

//        /// <summary>
//        /// Возвращает строку, если есть свойство с данным ключом. Если ключа нет, возвращается null.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public string GetStringOrNull(string name)
//        {
//            return this[name];
//        }

//        /// <summary>
//        /// Возвращает строку, если есть свойство с данным ключом. Если ключа нет, кидает исключение KeyNotFoundException.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public string GetString(string name)
//        {
//            if (Properties.ContainsKey(name))
//            {
//                return this[name];
//            }
//            throw new KeyNotFoundException();
//        }

//        #endregion

//        #region Binary

//        public void Set(string name, byte[] value)
//        {
//            this[name] = value;
//        }

//        /// <summary>
//        /// Возвращает строку, если есть свойство с данным ключом. Если ключа нет, возвращается null.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public byte[] GetBinaryOrNull(string name)
//        {
//            return this[name];
//        }

//        /// <summary>
//        /// Возвращает строку, если есть свойство с данным ключом. Если ключа нет, кидает исключение KeyNotFoundException.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public byte[] GetBinary(string name)
//        {
//            if (Properties.ContainsKey(name))
//            {
//                return this[name];
//            }
//            throw new KeyNotFoundException();
//        }

//        #endregion

//        #region collection methods

//        public IEnumerator<ExtentionProperty> GetEnumerator()
//        {
//            return Properties.Values.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return Properties.Values.GetEnumerator();
//        }

//        public void Add(ExtentionProperty item)
//        {
//            if (item == null)
//            {
//                throw new ArgumentNullException("item");
//            }
//            if (Properties.ContainsKey(item.Name))
//            {
//                Properties[item.Name] = item;
//            }
//            else
//            {
//                Properties.Add(item.Name, item);
//            }
//        }

//        public void Clear()
//        {
//            Properties.Clear();
//        }

//        public bool Contains(ExtentionProperty item)
//        {
//            return Properties.ContainsKey(item.Name);
//        }

//        public void CopyTo(ExtentionProperty[] array, int arrayIndex)
//        {
//            foreach (ExtentionProperty property in Properties.Values)
//            {
//                array[arrayIndex++] = property;
//            }
//        }

//        public int Count
//        {
//            get { return Properties.Count; }
//        }

//        public bool IsReadOnly
//        {
//            get { return false; }
//        }

//        public bool Remove(ExtentionProperty item)
//        {
//            if (Properties.ContainsKey(item.Name))
//            {
//                Properties.Remove(item.Name);
//                return true;
//            }
//            return false;
//        }

//        #endregion

//        public bool HasKey(string name)
//        {
//            return Properties.ContainsKey(name);
//        }

//        public ExtentionPropertyCollection() { }

//        public ExtentionPropertyCollection(IDictionary<string, object> properties)
//        {
//            foreach (var property in properties)
//            {
//                Set(property.Key, property.Value);
//            }
//        }

//        public XmlSchema GetSchema()
//        {
//            return null;
//        }

//        public void ReadXml(XmlReader reader)
//        {
//            if (reader.IsEmptyElement)
//            {
//                return;
//            }
//            reader.Read();
//            while (reader.Name == "Property")
//            {
//                reader.Read();
//                string name = reader.ReadElementString("Name");
//                string type = reader.ReadElementString("Type");
//                var value = reader.ReadElementString("Value");
//                var property = ApiConverter.GetExtentionPropertyFromXml(name, type, value);
//                Properties[name] = property;
//                reader.Read();
//            }
//            reader.Read();
//        }

//        public void WriteXml(XmlWriter writer)
//        {
//            foreach (var property in Properties.Values)
//            {
//                string xmlValue = ApiConverter.GetXmlValue(property.Value);
//                writer.WriteStartElement("Property");
//                writer.WriteElementString("Name", property.Name);
//                writer.WriteElementString("Type", property.Value.DataType.ToString());
//                writer.WriteElementString("Value", xmlValue);
//                writer.WriteEndElement();
//            }
//        }

//        public void CopyFrom(ExtentionPropertyCollection properties)
//        {
//            if (properties != null)
//            {
//                foreach (var property in properties)
//                {
//                    Add(property);
//                }
//            }
//        }
//    }
//}

