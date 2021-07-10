using System;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Исключение - объект уже удалён
    /// </summary>
    public class AlreadyDeletedException : UserFriendlyException
    {
        public AlreadyDeletedException(Guid id, INaming entity)
            : base(GetMessage(id, entity))
        {
        }

        protected static string GetMessage(Guid id, INaming entity)
        {
            return string.Format("Нет смысла удалять {0} {1}, который уже удалён", entity.Nominative(), id);
        }
    }
}