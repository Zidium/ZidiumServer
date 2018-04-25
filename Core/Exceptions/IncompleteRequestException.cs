using System;

namespace Zidium.Core
{
    public class IncompleteRequestException : Exception
    {
        private int _bodyLength;

        private int _contentLength;

        public IncompleteRequestException(int bodyLength, int contentLength)
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
