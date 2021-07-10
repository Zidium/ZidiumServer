using Zidium.Storage;

namespace Zidium.Core
{
    public static class SubscriptionHelper
    {
        public static SubscriptionChannel[] AvailableSubscriptionChannels { get; } = {
            SubscriptionChannel.Email,
            SubscriptionChannel.Telegram,
            SubscriptionChannel.VKontakte
        };
    }
}
