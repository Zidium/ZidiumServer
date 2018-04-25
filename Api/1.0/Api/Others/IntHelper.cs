using System;

namespace Zidium.Api
{
    internal static class IntHelper
    {
        public static bool TryParse(string s, out int result)
        {
            try
            {
                result = int.Parse(s);
                return true;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
        }
    }
}
