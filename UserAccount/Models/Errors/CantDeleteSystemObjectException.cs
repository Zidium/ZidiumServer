using System;
using Zidium.Common;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Исключение - нельзя редактировать системный объект
    /// </summary>
    public class CantDeleteSystemObjectException : UserFriendlyException
    {
        public CantDeleteSystemObjectException(Guid id, INaming entity)
            : base(GetMessage(id, entity))
        {
        }

        protected static string GetMessage(Guid id, INaming entity)
        {
            return string.Format("Нельзя удалять системный {0} {1}", entity.Nominative(), id);
        }
    }
}