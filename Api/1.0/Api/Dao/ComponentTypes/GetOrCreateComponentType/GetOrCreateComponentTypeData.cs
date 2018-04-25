using System;

namespace Zidium.Api
{
    /// <summary>
    /// Сообщение на регистрацию пользовательского типа компонента
    /// </summary>
    public class GetOrCreateComponentTypeData
    {
        public GetOrCreateComponentTypeData(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            SystemName = systemName;
        }

        public GetOrCreateComponentTypeData()
        {
            
        }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
