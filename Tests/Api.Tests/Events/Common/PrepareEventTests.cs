﻿using System;
using System.Linq;
using Zidium.Api.Others;
using Xunit;
using Zidium.TestTools;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Api.Tests.Events
{
    /// <summary>
    /// Проверим, что длинные свойства обрезаются
    /// </summary>

    public class PrepareEventTests : BaseTest
    {
        [Fact]
        public void PrepareDataHelperTest()
        {
            var client = new Client();
            var root = client.GetRootComponentControl();
            var helper = new PrepareDataHelper(client);
            string typeSystemName = new String('s', 1000);

            var eventData = root.CreateApplicationError(typeSystemName);
            eventData.Count = -10;
            eventData.Importance = null;
            eventData.JoinInterval = null;
            eventData.JoinKey = null;
            eventData.Message = new String('m', 1000 * 1000 * 10);
            eventData.StartDate = null;
            eventData.TypeDisplayName = new string('d', 1000);
            eventData.Version = "1";

            for (int i = 0; i < 10; i++)
            {
                eventData.Properties["text_" + Ulid.NewUlid()] = new String('t', 1000 * 1000 + i);
            }
            eventData.Properties["veryLongKey_" + new string('l', 1000)] = new DateTime(2015, 05, 09, 10, 00, 00);
            eventData.Properties["Stack"] = "stack value";

            client.Config.Events.DefaultValues.ApplicationError.JoinInterval = TimeSpan.FromMinutes(8);

            helper.PrepareEvent(eventData);

            // < 1 заменяется на 1
            Assert.Equal(1, eventData.Count);

            // EndDate может быть пустым
            //Assert.Null(eventData.EndDate);

            // если не установлено, берем из конфига
            Assert.Null(eventData.Importance);

            // если не установлен, берем из конфига
            Assert.Equal(eventData.JoinInterval, TimeSpan.FromMinutes(8));

            // если JoinKey не установлен, то вычисляется автоматически
            var joinKey = eventData.TypeCode + eventData.Message;
            foreach (var prop in eventData.Properties)
                joinKey += prop.Name + prop.Value.Value;
            Assert.Equal(eventData.JoinKey, HashHelper.GetInt64(joinKey));

            // обрезаем Message до 4000 символов
            Assert.Equal(eventData.Message, new string('m', 4000));

            // StartDate устанавливатся в текущую дату
            Assert.Equal(eventData.StartDate.Value.Date, DateTime.Now.Date);

            // TypeDisplayName обрезаем до 255 символов
            Assert.Equal(eventData.TypeDisplayName, new string('d', 255));

            // TypeSystemName обрезаем до 255 символов
            Assert.Equal(eventData.TypeSystemName, new string('s', 255));

            // должна быть хотя бы одна точка (добавили ".0")
            Assert.Equal("1.0", eventData.Version);

            // остались 5 text_ + стек  + veryLongKey_
            Assert.Equal(7, eventData.Properties.Count);
            int sum = eventData.Properties.Sum(x => x.Value.ToString().Length);
            Assert.True(sum > 5000 * 1000);

            // длинное имя свойства обрезалось
            var veryLongProperty = eventData.Properties.Single(x => x.Name.StartsWith("veryLongKey_"));
            Assert.Equal(100, veryLongProperty.Name.Length);
        }

        [Fact]
        public void PrepareDataHelper2Test()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var client = component.Client;
            string typeSystemName = new String('s', 1000);

            var eventData = component.CreateApplicationError(typeSystemName);
            eventData.Count = -10;
            eventData.Importance = null;
            eventData.JoinInterval = null;
            eventData.JoinKey = null;
            eventData.Message = new String('m', 1000 * 1000 * 10);
            eventData.StartDate = null;
            eventData.TypeDisplayName = new string('d', 1000);
            eventData.Version = "1";
            for (int i = 0; i < 10; i++)
            {
                eventData.Properties["text_" + Ulid.NewUlid()] = new String('t', 1000 * 1000 + i);
            }
            eventData.Properties["veryLongKey_" + new string('l', 1000)] = new DateTime(2015, 05, 09, 10, 00, 00);
            eventData.Properties["Stack"] = "stack value";

            client.Config.Events.DefaultValues.ApplicationError.JoinInterval = TimeSpan.FromMinutes(8);

            var response = eventData.Send();
            Assert.True(response.Success, response.ErrorMessage);

            // проверим, что перед отправкой событие обрезалось

            // < 1 заменяется 1
            Assert.Equal(1, eventData.Count);

            // EndDate может быть пустым
            //Assert.Null(eventData.EndDate);

            Assert.Null(eventData.Importance);

            // если не установлен, берем из конфига
            Assert.Equal(eventData.JoinInterval, TimeSpan.FromMinutes(8));

            // если JoinKey не установлен, то вычисляется автоматически
            var joinKey = eventData.TypeCode + eventData.Message;
            foreach (var prop in eventData.Properties)
                joinKey += prop.Name + prop.Value.Value;
            Assert.Equal(eventData.JoinKey, HashHelper.GetInt64(joinKey));

            // обрезаем Message до 4000 символов
            Assert.Equal(eventData.Message, new string('m', 4000));

            // StartDate устанавливатся в текущую дату
            Assert.Equal(eventData.StartDate.Value.Date, DateTime.Now.Date);

            // TypeDisplayName обрезаем до 255 символов
            Assert.Equal(eventData.TypeDisplayName, new string('d', 255));

            // TypeSystemName обрезаем до 255 символов
            Assert.Equal(eventData.TypeSystemName, new string('s', 255));

            // должна быть хотя бы одна точка (добавили ".0")
            Assert.Equal("1.0", eventData.Version);

            // остались 5 text_ + стек  + veryLongKey_
            Assert.Equal(7, eventData.Properties.Count);
            int sum = eventData.Properties.Sum(x => x.Value.ToString().Length);
            Assert.True(sum > 5000 * 1000);

            // длинное имя свойства обрезалось
            var veryLongProperty = eventData.Properties.Single(x => x.Name.StartsWith("veryLongKey_"));
            Assert.Equal(100, veryLongProperty.Name.Length);

            // проверим, что событие сохранилось в БД обрезанным
            var eventResponse = client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();

            Assert.Equal(EventCategory.ApplicationError, eventResponse.Category);
            Assert.Equal(eventResponse.OwnerId, component.Info.Id);
            Assert.Equal(1, eventResponse.Count);
            Assert.Equal(eventResponse.TypeDisplayName, new string('d', 255));
            Assert.Equal(eventResponse.TypeId, response.GetDataAndCheck().EventTypeId);
            Assert.False(eventResponse.IsUserHandled);
            Assert.Equal("1.0", eventResponse.Version);
            Assert.Equal(eventResponse.JoinKeyHash, HashHelper.GetInt64(joinKey));
            Assert.Equal(eventResponse.Message, new string('m', 4000));
            Assert.Equal(eventResponse.StartDate.Date, DateTime.Now.Date);

            // остались 5 text_ + стек  + veryLongKey_
            Assert.Equal(7, eventResponse.Properties.Count);
            sum = eventResponse.Properties.Sum(x => x.Value.Length);
            Assert.True(6000 * 1000 > sum);
            Assert.True(5000 * 1000 < sum);

            // длинное имя свойства обрезалось
            var cuttedVeryLongProperty = eventResponse.Properties.Single(x => x.Name.StartsWith("veryLongKey_"));
            Assert.Equal(100, veryLongProperty.Name.Length);
        }
    }
}
