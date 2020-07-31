using Zidium.Common;

namespace Zidium.Core.Common
{
    public class EmptyParameterException : UserFriendlyException
    {
        public EmptyParameterException(string parameter)
            : base("Параметр " + parameter + " не содержит значения")
        {

        }

        public static void Validate(string parameterName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EmptyParameterException(parameterName);
            }
        }
    }
}
