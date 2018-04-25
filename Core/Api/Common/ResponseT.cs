using Zidium.Api;

namespace Zidium.Core.Api
{
    public class ResponseT<TResponseData> : Response
    {
        public TResponseData InternalData { get; set; }

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
        }

        public new void Check()
        {
            if (!Success)
            {
                throw new ResponseException(this);
            }
        }
    }
}
