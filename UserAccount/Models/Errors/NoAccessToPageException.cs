using Zidium.Common;
using Zidium.Core;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Нет доступа к странице
    /// </summary>
    public class NoAccessToPageException : UserFriendlyException
    {
        public NoAccessToPageException() : base("Извините, у вас нет доступа") { }
    }
}