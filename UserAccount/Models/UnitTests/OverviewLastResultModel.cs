using System;
using System.Linq;
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
        public string ShowDetailsUrl { get; set; }
        public Guid? EventId { get; set; }

        /// <summary>
        /// Если проверка выключена, то нельзя выполнить проверку
        /// </summary>
        public bool ShowRunButton { get; set; }

        private static string GetShowDetailsUrl(UnitTest unitTest, Event eventObj)
        {
            if (unitTest.TypeId == SystemUnitTestTypes.VirusTotalTestType.Id)
            {
                var property = eventObj.Properties.FirstOrDefault(x => x.Name == "Permalink");
                if (property != null)
                {
                    return property.Value;
                }
            }
            return null;
        }

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
                model.EventId = eventObj.Id;
                model.ShowDetailsUrl = GetShowDetailsUrl(unitTest, eventObj);
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