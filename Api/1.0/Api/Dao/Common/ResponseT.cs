namespace Zidium.Api
{
    public class ResponseT<TResponseData> : Response
    {
        public TResponseData InternalData;

        public TResponseData Data
        {
            get
            {
                if (Success)
                {
                    return InternalData;
                }
                throw new ResponseException(this);
            }
            set { InternalData = value; }
        }
    }
}
