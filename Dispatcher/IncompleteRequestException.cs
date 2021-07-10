using System;

namespace Zidium.Dispatcher
{
    public class IncompleteRequestException : Exception
    {
        private long _bodyLength;

        private long _contentLength;

        public IncompleteRequestException(long bodyLength, long contentLength)
        {
            _bodyLength = bodyLength;
            _contentLength = contentLength;
        }

        public override string Message
        {
            get { return string.Format("Incomplete request, BodyLength: {0}, ContentLength: {1}", _bodyLength, _contentLength); }
        }

    }
}
