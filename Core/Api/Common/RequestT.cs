namespace Zidium.Core.Api
{
    public class RequestT<TRequestData> : Request
    {
        public TRequestData Data { get; set; }
    }
}
