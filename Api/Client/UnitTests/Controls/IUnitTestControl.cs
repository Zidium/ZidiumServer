using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public interface IUnitTestControl : IObjectControl
    {
        UnitTestDto Info { get; }

        IUnitTestTypeControl Type { get; }

        SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus);

        SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, string message);

        SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, TimeSpan actualTime);

        SendUnitTestResultResponseDto SendResult(UnitTestResult resultStatus, TimeSpan actualTime, string message);

        SendUnitTestResultResponseDto SendResult(SendUnitTestResultData data);

        GetUnitTestStateResponseDto GetState();

        SetUnitTestEnableResponseDto Enable();

        SetUnitTestDisableResponseDto Disable();

        SetUnitTestDisableResponseDto Disable(SetUnitTestDisableRequestData data);
    }
}
