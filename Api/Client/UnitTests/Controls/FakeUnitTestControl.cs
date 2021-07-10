using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class FakeUnitTestControl : IUnitTestControl
    {
        public void Dispose()
        {
        }

        public IClient Client { get { return null; } }

        public string SystemName { get; protected set; }

        public bool IsFake()
        {
            return true;
        }

        public void Detach()
        {
        }

        public UnitTestDto Info { get { return null; } }

        public IUnitTestTypeControl Type { get { return null; } }

        public SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, TimeSpan actualTime)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, TimeSpan actualTime, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public SendUnitTestResultResponseDto SendResult(SendUnitTestResultData data)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponseDto>();
        }

        public GetUnitTestStateResponseDto GetState()
        {
            return ResponseHelper.GetOfflineResponse<GetUnitTestStateResponseDto>();
        }

        public SetUnitTestDisableResponseDto Disable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponseDto>();
        }

        public SetUnitTestDisableResponseDto Disable(SetUnitTestDisableRequestData data)
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponseDto>();
        }

        public SetUnitTestEnableResponseDto Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestEnableResponseDto>();
        }
    }
}
