using System;

namespace Zidium.Api
{
    public abstract class SendEventBaseT<T> :SendEventBase
        where T : SendEventBase
    {
        protected SendEventBaseT(IComponentControl componentControl, string typeSystemName)
            : base(componentControl, typeSystemName)
        {
            
        }

        public T CreateCopy()
        {
            return CreateBaseCopy() as T;
        }

        public T SetImportance(EventImportance value)
        {
            Importance = value;
            return this as T;
        }

        public T SetJoinInterval(TimeSpan value)
        {
            JoinInterval = value;
            return this as T;
        }

        public T SetStartDate(DateTime value)
        {
            StartDate = value;
            return this as T;
        }

        public T SetJoinKey(params string[] parameters)
        {
            JoinKey = HashHelper.GetInt64(parameters);
            return this as T;
        }

        public T SetJoinKey(long value)
        {
            JoinKey = value;
            return this as T;
        }

        #region set extention properties

        public T SetProperty(string name, string value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, int value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, long value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, DateTime value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, double value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, Guid value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, bool value)
        {
            Properties[name] = value;
            return this as T;
        }

        public T SetProperty(string name, byte[] value)
        {
            Properties[name] = value;
            return this as T;
        }

        #endregion
    }
}
