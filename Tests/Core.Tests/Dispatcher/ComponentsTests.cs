﻿using System;
using System.Linq;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Api.Dto;
using Zidium.TestTools;
using Zidium.Common;

namespace Zidium.Core.Tests.Dispatcher
{
    public class ComponentsTests : BaseTest
    {
        [Fact]
        public void CreateDeleteCreateTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            
            // Создадим компонент
            var systemName = "Component." + Ulid.NewUlid();
            var getOrCreateComponentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                TypeId = SystemComponentType.WebSite.Id,
                SystemName = systemName,
                DisplayName = systemName,
                ParentComponentId = account.RootId
            });
            var component = getOrCreateComponentResponse.GetDataAndCheck();

            // Удалим его
            dispatcher.DeleteComponent(component.Component.Id);

            // Снова создадим с тем же системным именем
            getOrCreateComponentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                TypeId = SystemComponentType.WebSite.Id,
                SystemName = systemName,
                DisplayName = systemName,
                ParentComponentId = account.RootId
            });

            // Проверим, что создание выполнилось успешно
            Assert.True(getOrCreateComponentResponse.Success);
        }

        /// <summary>
        /// Был баг, когда выключение компонента не ставило флаг "ParentEnable==false" для проверок
        /// Данный тест проверяет, что флаг ParentEnable установится для дочерних проверок, метрик и детей
        /// </summary>
        [Fact]
        public void DisableComponentWithChilds()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("myTest");
            var metricType = TestHelper.CreateHddMetricType();
            var child = component.GetOrCreateChildComponentControl("type", "instance");
            Assert.False(child.IsFake());

            // проверим что компонент имеет серый цвет (нет данных)
            var componentState = component.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Unknown, componentState.Status);
           
            // HDD = 100 сделает метрику зеленой 
            component.SendMetric(metricType.SystemName, 100).Check();
            componentState = component.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Success, componentState.Status);

            // сделаем юнит-тест желтым
            unitTest.SendResult(UnitTestResult.Warning, TimeSpan.FromDays(1)).Check();
            componentState = component.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Warning, componentState.Status);

            // сделаем ребенка зеленым
            child.CreateComponentEvent("event")
               .SetImportance(EventImportance.Success)
               .Send()
               .Check();

            componentState = child.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Success, componentState.Status);

            // выключим компонент
            component.Disable("хз");
            account.SaveAllCaches();

            // проверим, что в БД все выключено
            // статусы объектов через АПИ проверим позже, т.к. они могут на лету изменить данные в БД
            using (var accountDbContext = account.GetDbContext())
            {
                var unitTestDb = accountDbContext.UnitTests.Find(unitTest.Info.Id);
                Assert.NotNull(unitTestDb);
                Assert.True(unitTestDb.Enable);
                Assert.False(unitTestDb.ParentEnable);
                Assert.Equal(MonitoringStatus.Disabled, unitTestDb.Bulb.Status);

                var componentDb = unitTestDb.Component;
                Assert.False(componentDb.Enable);
                Assert.True(componentDb.ParentEnable);
                Assert.Equal(MonitoringStatus.Disabled, componentDb.ExternalStatus.Status);
                Assert.Equal(MonitoringStatus.Disabled, componentDb.InternalStatus.Status);

                var metric = componentDb.Metrics.Single();
                Assert.True(metric.Enable);
                Assert.False(metric.ParentEnable);
                Assert.Equal(MonitoringStatus.Disabled, metric.Bulb.Status);
            }

            // статус компонента должен стать Disabled
            componentState = component.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Disabled, componentState.Status);

            // статус проверки должен стать Disabled
            var unitTestStatus = unitTest.GetState().GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Disabled, unitTestStatus.Status);

            // статус метрики должен стать Disabled
            var metricData = component.GetMetric(metricType.SystemName).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Disabled, metricData.Status);

            // включим компонент
            component.Enable().Check();
            account.SaveAllCaches();
            component.GetTotalState(true);

            // проверим, что в БД все включено
            using (var accountDbContext = account.GetDbContext())
            {
                var unitTestDb = accountDbContext.UnitTests.Find(unitTest.Info.Id);
                Assert.NotNull(unitTestDb);
                Assert.True(unitTestDb.Enable);
                Assert.True(unitTestDb.ParentEnable);
                Assert.Equal(MonitoringStatus.Unknown, unitTestDb.Bulb.Status);

                var componentDb = unitTestDb.Component;
                Assert.True(componentDb.Enable);
                Assert.True(componentDb.ParentEnable);
                Assert.Equal(MonitoringStatus.Success, componentDb.ExternalStatus.Status);
                Assert.Equal(MonitoringStatus.Unknown, componentDb.InternalStatus.Status);

                var metric = componentDb.Metrics.Single();
                Assert.True(metric.Enable);
                Assert.True(metric.ParentEnable);
                Assert.Equal(MonitoringStatus.Unknown, metric.Bulb.Status);
            }

            // статус компонента должен стать Success
            componentState = component.GetTotalState(false).GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Success, componentState.Status);

            // статус проверки должен стать Unknown
            unitTestStatus = unitTest.GetState().GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Unknown, unitTestStatus.Status);

        }

        /// <summary>
        /// Тест проверяет, что компонент удаляется вместе с детьми
        /// </summary>
        [Fact]
        public void DeleteWithChilds()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим компонент
            var systemName = "Component." + Ulid.NewUlid();
            var getOrCreateComponentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                TypeId = SystemComponentType.WebSite.Id,
                SystemName = systemName,
                DisplayName = systemName,
                ParentComponentId = account.RootId
            });
            var component = getOrCreateComponentResponse.GetDataAndCheck();

            // Создадим дочерний компонент
            var childSystemName = "Component." + Ulid.NewUlid();
            var getOrCreateChildComponentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                TypeId = SystemComponentType.WebSite.Id,
                SystemName = childSystemName,
                DisplayName = childSystemName,
                ParentComponentId = component.Component.Id
            });
            var child = getOrCreateChildComponentResponse.GetDataAndCheck();

            // Удалим основной компонент
            dispatcher.DeleteComponent(component.Component.Id);

            // Проверим, что оба компонента удалены
            using (var context = TestHelper.GetDbContext())
            {
                var componentFromDb = context.Components.Find(component.Component.Id);
                Assert.True(componentFromDb.IsDeleted);

                var childFromDb = context.Components.Find(child.Component.Id);
                Assert.True(childFromDb.IsDeleted);
            }
        }

        [Fact]
        public void DefaultWebLogLevelsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим компонент
            var systemName = "Component." + Ulid.NewUlid();
            var getOrCreateComponentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                TypeId = SystemComponentType.WebSite.Id,
                SystemName = systemName,
                DisplayName = systemName,
                ParentComponentId = account.RootId
            });
            var component = getOrCreateComponentResponse.GetDataAndCheck();

            // Проверим, что у ногого компонента выключены уровни лога Debug и Trace
            Assert.True(component.WebLogConfig.IsFatalEnabled);
            Assert.True(component.WebLogConfig.IsErrorEnabled);
            Assert.True(component.WebLogConfig.IsWarningEnabled);
            Assert.True(component.WebLogConfig.IsInfoEnabled);
            Assert.False(component.WebLogConfig.IsDebugEnabled);
            Assert.False(component.WebLogConfig.IsTraceEnabled);
        }

    }
}
