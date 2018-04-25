using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Имена объектов
    /// </summary>
    public static class Naming
    {
        public static INaming Component
        {
            get { return new ComponentNaming(); }
        }

        public static INaming ComponentProperty
        {
            get { return new ComponentPropertyNaming(); }
        }

        public static INaming ComponentState
        {
            get { return new ComponentStateNaming(); }
        }

        public static INaming ComponentType
        {
            get { return new ComponentTypeNaming(); }
        }

        public static INaming EventType
        {
            get { return new EventTypeNaming(); }
        }

        public static INaming Problem
        {
            get { return new ProblemNaming(); }
        }

        public static INaming Subscription
        {
            get { return new SubscriptionNaming(); }
        }

        public static INaming User
        {
            get { return new UserNaming(); }
        }

        public static INaming UserContact
        {
            get { return new UserContactNaming(); }
        }

        public static INaming HttpRequestUnitTestRule
        {
            get { return new HttpRequestUnitTestRuleNaming(); }
        }

        public static INaming UnitTest
        {
            get { return new UnitTestNaming(); }
        }

        public static INaming UnitTestType
        {
            get { return new UnitTestTypeNaming(); }
        }

        public static INaming Event
        {
            get { return new EventNaming(); }
        }

        public static INaming Log
        {
            get { return new LogNaming(); }
        }

        public static INaming Notification
        {
            get { return new NotificationNaming(); }
        }

        public static INaming TariffLimit
        {
            get { return new TariffLimitNaming(); }
        }

        public static INaming Counter
        {
            get { return new CounterNaming(); }
        }

        public static INaming StatusData
        {
            get { return new StatusDataNaming(); }
        }
    }
}
