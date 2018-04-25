using System;

namespace Zidium.Api
{
    public class UnitTestControlOnline : UnitTestControlBase
    {
        public UnitTestControlOnline(IComponentControl component, IUnitTestTypeControl type, UnitTestInfo info)
            : base(component, type, info.SystemName)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            InfoInternal = info;
        }
        

        public override bool IsFake()
        {
            return false;
        }

        public override SendUnitTestResultResponse SendResult(SendUnitTestResultData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendUnitTestResult(Info.Id, data);
        }

        public override SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SetUnitTestDisable(Info.Id, data);
        }

        protected UnitTestInfo InfoInternal { get; set; }

        public override UnitTestInfo Info
        {
            get { return InfoInternal; }
        }

        public override GetUnitTestStateResponse GetState()
        {
            return Client.ApiService.GetUnitTestState(Info.Id);
        }
        

        public override SetUnitTestEnableResponse Enable()
        {
            return Client.ApiService.SetUnitTestEnable(Info.Id);
        }
    }
}
