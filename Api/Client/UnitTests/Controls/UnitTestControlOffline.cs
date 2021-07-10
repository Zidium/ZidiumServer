using Zidium.Api.Dto;

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

        public override GetUnitTestStateResponseDto GetState()
        {
            return ResponseHelper.GetOfflineResponse<GetUnitTestStateResponseDto>();
        }

        public override SetUnitTestEnableResponseDto Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestEnableResponseDto>();
        }

        public override SetUnitTestDisableResponseDto Disable(SetUnitTestDisableRequestData data)
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponseDto>();
        }


        public override bool IsFake()
        {
            return true;
        }

        public override SendUnitTestResultResponseDto SendResult(SendUnitTestResultData data)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public override UnitTestDto Info
        {
            get { return null; }
        }

        protected IUnitTestTypeControl TypeInternal { get; set; }

    }
}
