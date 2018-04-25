namespace Zidium.Api
{
    public class UnitTestControlOffline : UnitTestControlBase
    {
        public UnitTestControlOffline(
            IComponentControl component,
            IUnitTestTypeControl type,
            string systemName) 
            : base(component, type, systemName)
        {
        }

        public override GetUnitTestStateResponse GetState()
        {
            return ResponseHelper.GetOfflineResponse<GetUnitTestStateResponse>();
        }

        public override SetUnitTestEnableResponse Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestEnableResponse>();
        }

        public override SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data)
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponse>();
        }
        

        public override bool IsFake()
        {
            return true;
        }

        public override SendUnitTestResultResponse SendResult(SendUnitTestResultData data)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public override UnitTestInfo Info
        {
            get { return null; }
        }

        protected IUnitTestTypeControl TypeInternal { get; set; }
        
    }
}
