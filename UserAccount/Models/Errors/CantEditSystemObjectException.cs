using System;
using Zidium.Common;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Исключение - нельзя редактировать системный объект
    /// </summary>
    public class CantEditSystemObjectException : UserFriendlyException
    {
        public CantEditSystemObjectException(Guid id, INaming entity)
            : base(GetMessage(id, entity))
        {
        }

        protected static string GetMessage(Guid id, INaming entity)
        {
            return string.Format("Нельзя редактировать системный {0} {1}", entity.Nominative(), id);
        }
    }
}