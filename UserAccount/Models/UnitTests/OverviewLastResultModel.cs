using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewLastResultModel
    {
        private const string ParamName = "eventObj";

        public Guid UnitTestId { get; set; }
        public MonitoringStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime? ExecutionTime { get; set; }

        /// <summary>
        /// Если проверка выключена, то нельзя выполнить проверку
        /// </summary>
        public bool ShowRunButton { get; set; }

        public static OverviewLastResultModel Create(UnitTest unitTest, Event eventObj)
        {
            var model = new OverviewLastResultModel()
            {
                UnitTestId = unitTest.Id,
                Status = unitTest.Bulb.Status,
                ShowRunButton = unitTest.Bulb.Status != Core.Api.MonitoringStatus.Disabled
            };
            if (eventObj == null)
            {
                model.Message = "<Нет выполнений>";
            }
            else
            {
                model.ExecutionTime = eventObj.StartDate;
                model.Message = eventObj.Message;
                if (model.Message == null)
                {
                    model.Message = "<Сообщение отсутствует>";
                }
            }
            return model;
        }
    }
}