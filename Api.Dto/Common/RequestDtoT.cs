namespace Zidium.Api.Dto
{
    public class RequestDtoT<TRequestData> : RequestDto
    {
        public TRequestData Data { get; set; }
    }
}
