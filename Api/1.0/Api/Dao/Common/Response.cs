namespace Zidium.Api
{
    public class Response
    {
        public bool Success
        {
            get { return Code == ResponseCode.Success; }
        }

        public int? Code { get; set; }

        public string ErrorMessage { get; set; }

        public void Check()
        {
            if (Success == false)
            {
                throw new ResponseException(this);
            }
        }
    }
}
