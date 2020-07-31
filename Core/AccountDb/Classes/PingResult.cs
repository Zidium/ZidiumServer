using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class PingResult
    {
        public PingErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
