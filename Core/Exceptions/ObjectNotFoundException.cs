using System;
using Zidium.Core.Common;

namespace Zidium.Core
{
    /// <summary>
    /// Исключение - объект не найден
    /// </summary>
    public class ObjectNotFoundException : UserFriendlyException
    {

        public ObjectNotFoundException(string message) : base(message)
        {
            
        }

        public ObjectNotFoundException(Guid id, INaming entity)
            : base(GetMessage(id, entity))
        {
        }

        public ObjectNotFoundException(Guid accountId, Guid id, INaming entity)
            : base(GetMessage(accountId, id, entity))
        {
        }

        protected static string GetMessage(Guid accountId, Guid id, INaming entity)
        {
            return string.Format("{0} {1} {2} в аккаунте {3}", entity.Nominative(), id, entity.NotFound(), accountId);
        }

        protected static string GetMessage(Guid id, INaming entity)
        {
            return string.Format("{0} {1} {2}", entity.Nominative(), id, entity.NotFound());
        }
    }
}