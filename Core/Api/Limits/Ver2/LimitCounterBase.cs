using System;
using System.Threading;
using Zidium.Core.Api.Limits.Ver2;
using Zidium.Core.Limits;

namespace Zidium.Core.Api.Limits
{
    public abstract class LimitCounterBase : ILimitCounter
    {
        protected long _CurrentValue;
        protected long _SoftLimit;
        protected long _HardLimit;
        protected IAccountLimitChecker _AccountLimitChecker;

        private const long NO_DATA = -1;

        public LimitCounterBase(IAccountLimitChecker accountLimitChecker)
        {
            if (accountLimitChecker == null)
            {
                throw new ArgumentNullException("accountLimitChecker");
            }
            _AccountLimitChecker = accountLimitChecker;
        }

        protected void SetAccountOverLimitSignal()
        {
            _AccountLimitChecker.SetAccountOverlimitSignal();
        }

        public void Check(long value)
        {
            long newValue = _CurrentValue + value;
            if (newValue > _SoftLimit)
            {
                SetAccountOverLimitSignal();
            }
            if (newValue > _HardLimit)
            {
                throw new OverLimitException("Превышен лимит"); //todo добвить в сообщение текущее значение, добавляемое и максимальное
            }    
        }

        public void Add(long value)
        {
            // обернем в try-catch, чтобы бизнес логика не пострадала от ошибок лимитов
            try
            {
                // используем шаблон 2-ой проверки
                if (_CurrentValue == NO_DATA)
                {
                    lock (this)
                    {
                        if (_CurrentValue == NO_DATA)
                        {
                            _CurrentValue = GetActualCurrentValue();
                        }
                    }
                }
                Interlocked.Add(ref _CurrentValue, value);
            }
            catch (Exception exception)
            {
                _AccountLimitChecker.ComponentControl.AddApplicationError(exception);
            }
        }

        public long Value
        {
            get { return _CurrentValue; }
        }

        public void Refresh()
        {
            _CurrentValue = NO_DATA;
        }

        protected abstract long GetActualCurrentValue();
    }
}
