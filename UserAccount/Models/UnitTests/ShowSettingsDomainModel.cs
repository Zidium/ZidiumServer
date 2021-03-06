﻿using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsDomainModel
    {
        public Guid UnitTestId { get; set; }
        public string Domain { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static ShowSettingsDomainModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.DomainNamePaymentPeriodRules.GetOneByUnitTestId(unitTest.Id);
            
            return new ShowSettingsDomainModel()
            {
                UnitTestId = unitTest.Id,
                Domain = rule.Domain,
                AlarmDays = rule.AlarmDaysCount,
                WarningDays = rule.WarningDaysCount
            };
        }
    }
}