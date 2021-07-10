using System;

namespace Zidium.Api.Tests.Events.ApplicationErrors
{
    internal static class TypeSystemNameHelper
    {
        public static void Throw(string a, string b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }
        }
    }
}
