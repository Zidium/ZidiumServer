using System;
using System.ComponentModel;
using System.Linq;

namespace Zidium.Core.Caching
{
    public class ObjectChangesHelper
    {
        public static bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(type.GetGenericArguments()[0]);
            }
            if (type.IsEnum)
            {
                return true;
            }
            var validTypes = new[]
            {
                typeof(int),
                typeof(long),
                typeof(bool),
                typeof(byte),
                typeof(Guid),
                typeof(DateTime),
                typeof(string)
            };
            return validTypes.Contains(type);
        }

        public static bool HasChanges<T>(T a, T b)
            where T: class 
        {
            if (a == null && b == null)
            {
                return false;
            }
            if (a == null || b == null)
            {
                return true;
            }
            var properties = typeof (T).GetProperties();
            
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    if (IsSimpleType(property.PropertyType))
                    {
                        object valA = property.GetValue(a);
                        object valB = property.GetValue(b);
                        if (valA == null && valB == null)
                        {
                            continue;
                        }
                        if (valA == null || valB == null)
                        {
                            return true;
                        }
                        if (Equals(valA, valB) == false)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
