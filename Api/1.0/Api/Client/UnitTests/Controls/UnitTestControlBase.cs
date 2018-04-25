using System;

namespace Zidium.Api
{
    public abstract class UnitTestControlBase : IUnitTestControl
    {
        public abstract UnitTestInfo Info { get; }

        public string SystemName { get; protected set; }

        public IClient Client
        {
            get { return Component.Client; }
        }

        public IUnitTestTypeControl Type { get; protected set; }

        public IComponentControl Component { get; protected set; }
        
        protected UnitTestControlBase(
            IComponentControl component, 
            IUnitTestTypeControl type,  
            string systemName)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            SystemName = systemName;//todo
            Component = component;
            Type = type;
        }

        public abstract GetUnitTestStateResponse GetState();

        public abstract bool IsFake();

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus)
        {
            var data = new SendUnitTestResultData()
            {
                Result = resultStatus
            };
            return SendResult(data);
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, string message)
        {
            var data = new SendUnitTestResultData()
            {
                Message = message,
                Result = resultStatus
            };
            return SendResult(data);
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime)
        {
            var data = new SendUnitTestResultData()
            {
                ActualInterval = actualTime,
                Result = resultStatus
            };
            return SendResult(data);
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime, string message)
        {
            var data = new SendUnitTestResultData()
            {
                ActualInterval = actualTime,
                Result = resultStatus,
                Message = message
            };
            return SendResult(data);
        }

        public abstract SendUnitTestResultResponse SendResult(SendUnitTestResultData data);

        public abstract SetUnitTestEnableResponse Enable();

        public SetUnitTestDisableResponse Disable()
        {
            return Disable(new SetUnitTestDisableRequestData()
            {
                // пустные значения
            });
        }

        public abstract SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data);
    }
}
