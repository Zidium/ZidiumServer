using System;

namespace Zidium.Core.Limits
{
    public class OverLimitException : Exception
    {
        public OverLimitException(string message)
            : base(message)
        {
        }
    }
}
