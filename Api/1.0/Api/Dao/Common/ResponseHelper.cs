namespace Zidium.Api
{
    public static class ResponseHelper
    {
        public static TResponse GetOfflineResponse<TResponse>() where TResponse : Response, new()
        {
            return new TResponse()
            {
                Code = ResponseCode.Offine,
                ErrorMessage = "Веб-сервис временно недоступен"
            };
        }

        public static TResponse GetClientErrorResponse<TResponse>(string errorMessage) where TResponse : Response, new()
        {
            return new TResponse()
            {
                Code = ResponseCode.ClientError,
                ErrorMessage = errorMessage
            };
        }
    }
}
