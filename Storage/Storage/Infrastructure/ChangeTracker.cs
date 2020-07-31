using System;

namespace Zidium.Storage
{
    public class ChangeTracker<T>
    {
        public ChangeTracker()
        {
            _value = default(T);
            _changed = false;
        }

        public ChangeTracker(T initialValue)
        {
            _value = initialValue;
            _changed = false;
        }

        private T _value;

        private bool _changed;

        public T Get()
        {
            if (!_changed)
                throw new Exception("Property never changed, original value is not available");

            return _value;
        }

        public void Set(T newValue)
        {
            _value = newValue;
            _changed = true;
        }

        public bool Changed()
        {
            return _changed;
        }
    }
}
