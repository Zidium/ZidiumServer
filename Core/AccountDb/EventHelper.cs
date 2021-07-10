using System;

namespace AppMonitoring.Core.AccountsDb
{
    public static class EventHelper
    {
        /// <summary>
        /// Идентификатор типа события "ApplicationError"
        /// </summary>
        public static readonly Guid ApplicationErrorEventTypeId = new Guid("901E1C95-7CCC-4DE5-A71B-6FA5366EAFF1");
    }
}
