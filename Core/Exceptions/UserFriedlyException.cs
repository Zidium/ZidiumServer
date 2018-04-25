using System;

namespace Zidium.Core
{
    public class UserFriendlyException : ZidiumException
    {
        public UserFriendlyException(string message)
            : base(message)
        {

        }

        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
