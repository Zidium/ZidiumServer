using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

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

        private static string GetShowDetailsUrl(UnitTestForRead unitTest, EventForRead eventObj, IStorage storage)
        {
            if (unitTest.TypeId == SystemUnitTestType.VirusTotalTestType.Id)
            {
                var property = storage.EventProperties.GetByEventId(eventObj.Id).FirstOrDefault(x => x.Name == "Permalink");
                if (property != null)
                {
                    return property.Value;
                }
            }
            return null;
        }

        public static OverviewLastResultModel Create(UnitTestForRead unitTest, EventForRead eventObj, IStorage storage)
        {
            var bulb = storage.Bulbs.GetOneById(unitTest.StatusDataId);
            var model = new OverviewLastResultModel()
            {
                UnitTestId = unitTest.Id,
                Status = bulb.Status,
                ShowRunButton = true,
                IsCustom = SystemUnitTestType.IsSystem(unitTest.TypeId) == false
            };
            if (bulb.Status == MonitoringStatus.Disabled)
            {
                model.ShowRunButton = false;
            }
            if (SystemUnitTestType.IsSystem(unitTest.TypeId) == false)
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
                model.ShowDetailsUrl = GetShowDetailsUrl(unitTest, eventObj, storage);
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