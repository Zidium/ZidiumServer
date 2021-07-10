using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestControlOnline : UnitTestControlBase
    {
        public UnitTestControlOnline(IComponentControl component, IUnitTestTypeControl type, UnitTestDto info)
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

        public override SendUnitTestResultResponseDto SendResult(SendUnitTestResultData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendUnitTestResult(new SendUnitTestResultRequestDataDto()
            {
                UnitTestId = Info.Id,
                Message = data.Message,
                ReasonCode = data.ReasonCode,
                Result = data.Result,
                ActualIntervalSeconds = data.ActualInterval?.TotalSeconds,
                Properties = data.Properties.ToDtoList()
            });
        }

        public override SetUnitTestDisableResponseDto Disable(SetUnitTestDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SetUnitTestDisable(new SetUnitTestDisableRequestDataDto()
            {
                UnitTestId = Info.Id,
                ToDate = data.ToDate,
                Comment = data.Comment
            });
        }

        protected UnitTestDto InfoInternal { get; set; }

        public override UnitTestDto Info
        {
            get { return InfoInternal; }
        }

        public override GetUnitTestStateResponseDto GetState()
        {
            return Client.ApiService.GetUnitTestState(Info.Id);
        }


        public override SetUnitTestEnableResponseDto Enable()
        {
            return Client.ApiService.SetUnitTestEnable(Info.Id);
        }
    }
}
