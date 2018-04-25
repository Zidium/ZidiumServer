namespace Zidium.Api.Dto
{
    public class RequestT<TRequestData> : Request
    {
        public TRequestData Data { get; set; }
    }
}
