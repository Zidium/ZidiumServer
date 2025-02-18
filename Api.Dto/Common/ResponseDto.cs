namespace Zidium.Api.Dto
{
    public class ResponseDto
    {
        public bool Success
        {
            get { return Code == ResponseCode.Success || Code == ResponseCode.ObjectDisabled; }
        }

        public int? Code { get; set; }

        public string ErrorMessage { get; set; }

        public void Check()
        {
            if (!Success)
            {
                throw new ResponseException(this);
            }
        }
    }
}
