using System;
using System.Linq;

namespace Zidium.Api.Others
{
    public class PrepareDataHelper
    {
        public readonly int SystemNameMaxLength = 255;
        public readonly int DisplayNameMaxLength = 255;
        public readonly int MessageMaxLength = 1000 * 4; // 4 KBytes
        public readonly int VersionMaxLength = 100;
        public readonly int PropertyNameMaxLength = 100;
        public readonly int AllPropertiesMaxLength = 1024 * 1024 * 5; // 5 MBytes


        public IClient Client { get; protected  set; }

        public PrepareDataHelper(IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Client = client;
        }

        /// <summary>
        /// Обрезает пробелы по краям и обрезает под максимальную длину
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        protected string GetGoodString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            value = value.Trim();
            StringHelper.SetMaxLength(ref value, maxLength);
            value = value.TrimEnd();
            return value;
        }

        protected string FixEventTypeSystemName(string typeSystemName, string typeDisplayName)
        {
            string value = typeSystemName;
            if (string.IsNullOrEmpty(value))
            {
                value = typeDisplayName;
            }
            if (string.IsNullOrEmpty(value))
            {
                value = "Unknown"; //todo
            }            
            return value;
        }

        protected string FixEventMessage(string message, string typeSystemName, string typeDisplayName)
        {
            string value = message;
            if (string.Equals(value, typeDisplayName))
            {
                value = null; // нет смысла передавать тоже самое
            }
            else if (string.Equals(value, typeSystemName))
            {
                value = null; // нет смысла передавать тоже самое
            }
            return value;
        }

        protected long FixEventJoinKey(SendEventBase data)
        {
            if (data.JoinKey.HasValue)
                return data.JoinKey.Value;

            var joinKey = data.TypeCode + data.Message;
            foreach (var prop in data.Properties)
                joinKey += prop.Name + prop.Value.Value;

            return HashHelper.GetInt64(joinKey);
        }

        protected TimeSpan FixEventJoinTime(TimeSpan? joinInterval, SendEventCategory eventCategory)
        {
            if (joinInterval.HasValue)
            {
                return joinInterval.Value;
            }
            if (eventCategory == SendEventCategory.ApplicationError)
            {
                // todo нужно вынести настройки в личный кабинет
                return Client.Config.Events.DefaultValues.ApplicationError.JoinInterval;
            }
            if (eventCategory == SendEventCategory.ComponentEvent)
            {
                return Client.Config.Events.DefaultValues.ComponentEvent.JoinInterval;
            }
            throw new Exception("Неизвестное значение Category: " + eventCategory);
        }

        protected string FixEventVersion(IComponentControl componentControl, string version)
        {
            string value = FixVersion(version);
            if (string.IsNullOrEmpty(value))
            {
                value = componentControl.Version;
            }
            return FixVersion(value);
        }

        protected void TrimProperties(ExtentionPropertyCollection properties)
        {
            TrimProperties(properties, AllPropertiesMaxLength);
        }

        protected void TrimProperty(ExtentionPropertyCollection properties, ExtentionProperty property)
        {
            string name = GetGoodString(property.Name, PropertyNameMaxLength);
            if (name == null)
            {
                properties.Remove(property);
                return;
            }
            if (name != property.Name)
            {
                properties.Rename(property, name);
            }
        }

        protected int FixEventCount(int? count)
        {
            if (count == null)
            {
                return 1;
            }
            if (count < 1)
            {
                return 1;
            }
            return count.Value;
        }

        protected void TrimProperties(ExtentionPropertyCollection properties,  int maxSize)
        {
            if (properties == null)
            {
                return;
            }
            var allProperties = properties.ToList();
            foreach (var property in allProperties)
            {
                TrimProperty(properties, property);
            }
            int size = properties.GetWebSize();
            while (size > maxSize)
            {
                if (properties.Count == 0)
                {
                    break;
                }

                //удаляем одно самое тяжелое свойство
                var heavyProperty = properties
                    .ToList()
                    .OrderByDescending(x => x.GetWebSize())
                    .First();

                properties.Remove(heavyProperty);
                size = properties.GetWebSize();
            }
        }

        public static string FixVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return null;
            }
            try
            {
                if (version.Contains('.')==false)
                {
                    version = version + ".0";
                }
                version = version.Trim();
                var ver = new Version(version);
                return ver.ToString();
            }
            catch
            {
                return null;
            }
        }

        public void PrepareEvent(SendEventBase eventBase)
        {
            var eventObj = eventBase;

            // нормализация данных
            eventObj.TypeDisplayName = GetGoodString(eventBase.TypeDisplayName, DisplayNameMaxLength);
            eventObj.TypeSystemName = GetGoodString(eventBase.TypeSystemName, SystemNameMaxLength);
            eventObj.Message = GetGoodString(eventBase.Message, MessageMaxLength);
            eventObj.Count = FixEventCount(eventObj.Count);

            // установим время, если оно не задано явно
            if (eventObj.StartDate == null)
            {
                eventObj.StartDate = DateTime.Now;
                eventObj.IsServerTime = false;
            }

            // обрежем слишком жирные свойства
            TrimProperties(eventObj.Properties);
           
            eventObj.TypeSystemName = FixEventTypeSystemName(eventObj.TypeSystemName, eventObj.TypeDisplayName);
            eventObj.JoinInterval = FixEventJoinTime(eventObj.JoinInterval, eventObj.EventCategory);
            eventObj.JoinKey = FixEventJoinKey(eventObj);
            eventObj.Message = FixEventMessage(eventObj.Message, eventObj.TypeSystemName, eventObj.TypeDisplayName);
            if (eventObj.Version == null)
            {
                eventObj.Version = eventObj.ComponentControl.Version;
            }
            eventObj.Version = FixVersion(eventObj.Version);
        }
    }
}
