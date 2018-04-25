//using System;

//namespace Zidium.Core.Api
//{
//    public class ResponseException : Exception
//    {
//        public Response Response { get; protected set; }

//        public ResponseException(int responseCode, string errorMessage)
//            : base(errorMessage)
//        {
//            Response = new Response()
//            {
//                Code = responseCode,
//                ErrorMessage = errorMessage
//            };
//        }

//        public ResponseException(Response response)
//            : base(response.ErrorMessage)
//        {
//            Response = response;
//        }

//        public ResponseException(Response response, Exception innerException)
//            : base(response.ErrorMessage, innerException)
//        {
//            Response = response;
//        }
//    }
//}
