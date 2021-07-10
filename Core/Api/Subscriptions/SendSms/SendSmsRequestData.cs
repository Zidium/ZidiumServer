using System;

namespace Zidium.Core.Api
{
    public class SendSmsRequestData
    {
        public string Phone { get; set; }

        public string Body { get; set; }

        public Guid? ReferenceId { get; set; }
    }
}
