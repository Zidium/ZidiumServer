using Zidium.Storage;

namespace Zidium.Core.Common.Helpers
{
    public static class SubscriptionObjectHelper
    {
        public static string GetName(SubscriptionObject @object)
        {
            if (@object == SubscriptionObject.Default)
            {
                return "по умолчанию";
            }
            if (@object == SubscriptionObject.ComponentType)
            {
                return "на тип компонента";
            }
            if (@object == SubscriptionObject.Component)
            {
                return "на компонент";
            }
            return "?";
        }

        public static string GetAddTitle(SubscriptionObject @object)
        {
            return "Добавление подписки " + GetName(@object);
        }

        public static string GetEditTitle(SubscriptionObject @object)
        {
            return "Настройка подписки " + GetName(@object);
        }
    }
}
