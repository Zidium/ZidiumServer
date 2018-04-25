using System;

namespace Zidium.Core
{
    public class WrongContentTypeException : Exception
    {
        private string _contentType;

        public WrongContentTypeException(string contentType)
        {
            _contentType = contentType;
        }

        public override string Message
        {
            get
            {
                return "Неизвестный ContentType: " + (_contentType ?? "null") + "\r\n" +
                    "Для json указывайте application/json";
            }
        }
    }
}
