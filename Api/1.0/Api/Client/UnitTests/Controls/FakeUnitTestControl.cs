using System;

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

        public UnitTestInfo Info { get { return null; } }

        public IUnitTestTypeControl Type { get { return null; } }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public SendUnitTestResultResponse SendResult(SendUnitTestResultData data)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultResponse>();
        }

        public GetUnitTestStateResponse GetState()
        {
            return ResponseHelper.GetOfflineResponse<GetUnitTestStateResponse>();
        }

        public SetUnitTestDisableResponse Disable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponse>();
        }

        public SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data)
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestDisableResponse>();
        }

        public SetUnitTestEnableResponse Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetUnitTestEnableResponse>();
        }
    }
}
