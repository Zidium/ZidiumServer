using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewLastResultModel
    {
        public Guid UnitTestId { get; set; }
        public MonitoringStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public bool IsCustom { get; set; }
        public bool HasExecution => ExecutionTime != null;

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
                ShowRunButton = true,
                IsCustom = SystemUnitTestTypes.IsSystem(unitTest.TypeId)==false
            };
            if (unitTest.Bulb.Status== Core.Api.MonitoringStatus.Disabled)
            {
                model.ShowRunButton = false;
            }
            if (SystemUnitTestTypes.IsSystem(unitTest.TypeId) == false)
            {
                model.ShowRunButton = false;
            }
            if (eventObj == null)
            {
                // не было выполнений
                model.ShowRunButton = false;
            }
            else
            {
                // были выполнения
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