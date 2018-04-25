using System;

namespace Zidium.Core.Api
{
    public class SendHttpUnitTestBannerRequestData
    {
        public Guid? UnitTestId { get; set; }

        public bool? HasBanner { get; set; }
    }
}
