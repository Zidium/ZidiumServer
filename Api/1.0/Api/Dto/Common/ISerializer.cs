using System;

namespace Zidium.Api.Dto
{
    public interface ISerializer
    {
        byte[] GetBytes(object obj);

        string GetString(object obj);

        object GetObject(Type type, byte[] bytes);

        string Format { get; }
    }
}
