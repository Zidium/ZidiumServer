using System;
using Zidium.Core.Common;

namespace Zidium.Core
{
    /// <summary>
    /// Исключение - нет доступа к объекту
    /// </summary>
    public class AccessDeniedException : UserFriendlyException
    {
        public AccessDeniedException(Guid id, INaming entity)
            : base(GetMessage(id, entity))
        {
        }

        protected static string GetMessage(Guid id, INaming entity)
        {
            return string.Format("Нет доступа к {0} {1}", entity.Dative(), id);
        }
    }
}
