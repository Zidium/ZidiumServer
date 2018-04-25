namespace Zidium.Api.Dto
{
    public class ResponseDtoT<TResponseData> : Response
    {
        public TResponseData Data { get; set; }
    }
}
