using System;

namespace Zidium.Core.Api
{
    public class Request
    {
        public string Ip { get; set; }

        public string ProgramName { get; set; }

        public Guid? RequestId { get; set; }

        public AccessToken Token { get; set; }
    }
}
