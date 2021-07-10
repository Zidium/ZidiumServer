namespace Zidium.Core.Caching
{
    public interface ICacheWriteObjectT<TResponse, TWriteObject>: ICacheWriteObject
    {
        TResponse Response { get; set; }

        TWriteObject GetCopy();
    }
}
