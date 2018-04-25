using System;
using System.Linq;
using System.Collections.Generic;

namespace Zidium.UserAccount.Helpers
{
    public static class EnumHelper
    {
        public static T? StringToEnum<T>(string value) where T: struct
        {
            T? result = null;
            if (!string.IsNullOrEmpty(value))
            {
                T t;
                if (Enum.TryParse(value, out t))
                    result = t;
            }
            return result;
        }

        public static string EnumToString<T>(T value)
        {
            if (Namings.ContainsKey(typeof(T)))
            {
                var naming = (IEnumNaming<T>)Namings[typeof(T)];
                return naming.Name(value);
            }
            return value.ToString();
        }

        public static void RegisterNaming<T>(IEnumNaming<T> naming)
        {
            Namings.Add(typeof(T), naming);
        }

        public static T[] StringToEnumArray<T>(string value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return new T[0];
            var array = value.Split('~');
            var result = array.Select(StringToEnum<T>).Where(t => t != null).Select(t => t.Value).ToArray();
            return result;
        }

        internal static Dictionary<Type, object> Namings = new Dictionary<Type, object>();

    }
}