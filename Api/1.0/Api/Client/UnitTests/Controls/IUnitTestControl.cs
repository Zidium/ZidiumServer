using System;

namespace Zidium.Api
{
    public interface IUnitTestControl : IObjectControl
    {
        UnitTestInfo Info { get; }

        IUnitTestTypeControl Type { get; }

        SendUnitTestResultResponse SendResult(UnitTestResult resultStatus);

        SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, string message);

        SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime);

        SendUnitTestResultResponse SendResult(UnitTestResult resultStatus, TimeSpan actualTime, string message);

        SendUnitTestResultResponse SendResult(SendUnitTestResultData data);

        GetUnitTestStateResponse GetState();

        SetUnitTestEnableResponse Enable();

        SetUnitTestDisableResponse Disable();

        SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data);
    }
}
