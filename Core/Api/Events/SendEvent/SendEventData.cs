using System;
using System.Collections.Generic;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Api
{
    public class SendEventData
    {
        public Guid? ComponentId { get; set; }

        /// <summary>
        /// Системное имя типа события
        /// </summary>
        public string TypeSystemName { get; set; }

        /// <summary>
        /// Отображаемое имя типа события
        /// </summary>
        public string TypeDisplayName { get; set; }

        /// <summary>
        /// Код события
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Описание события
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Дата, когда событие началось
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Количество случаев события
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Важность
        /// </summary>
        public EventImportance? Importance { get; set; }

        /// <summary>
        /// Ключ для склеивания событий
        /// </summary>
        public long? JoinKey { get; set; }

        public double? JoinInterval { get; set; }

        /// <summary>
        /// Категория события
        /// </summary>
        public EventCategory? Category { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public List<ExtentionPropertyDto> Properties { get; set; }

        public long GetSize()
        {
            long size = DataSizeHelper.DbEventRecordOverhead;
            if (Message != null)
            {
                size += Message.Length * sizeof(char);
            }
            if (Version != null)
            {
                size += Version.Length * sizeof(char);
            }
            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    if (property != null)
                    {
                        size += DataSizeHelper.DbEventParameterRecordOverhead + property.GetSize();
                    }
                }
            }
            size += DataSizeHelper.DbEventStatusRecordOverhead;
            return size;
        }
    }
}
