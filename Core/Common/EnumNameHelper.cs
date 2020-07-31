using System;
using Zidium.Core.Common.EnumNames;

namespace Zidium.Core.Common
{
    public static class EnumNameHelper
    {
        public static IEnumName Get(Language language)
        {
            if (language == Language.Russian)
            {
                return new RussianEnumName();
            }
            throw new Exception("Неизвестный язык " + language);
        }
    }
}
