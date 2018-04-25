using System;

namespace ApiTests_1._0.Events.ApplicationErrors
{
    class TypeSystemNameHelper
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
