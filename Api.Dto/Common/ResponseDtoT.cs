namespace Zidium.Api.Dto
{
    public class ResponseDtoT<TResponseData> : ResponseDto
    {
        public TResponseData Data;

        public TResponseData GetDataAndCheck()
        {
            Check();
            return Data;
        }
    }
}
