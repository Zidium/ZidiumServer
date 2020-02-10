using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.HttpRequests;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.ConfigDb;
using Zidium.Core.Limits;
using Xunit;
using Zidium.Core.Common.TimeService;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class TariffLimitTests : IDisposable
    {
        [Fact]
        public void ComponentsCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим лимит аккаунта
            const int componentsMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.ComponentsMax = limits.UsedInstant.ComponentsCount + componentsMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var componentType = TestHelper.CreateRandomComponentTypeId(account.Id);
            var componentId = Guid.Empty;

            // Проверим, что можно создавать компоненты до лимита
            for (var i = 0; i < componentsMax; i++)
            {
                var request = new GetOrCreateComponentRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateComponentRequestData()
                    {
                        SystemName = "Component." + Guid.NewGuid(),
                        TypeId = componentType
                    }
                };
                var response = dispatcher.GetOrCreateComponent(request);
                Assert.True(response.Success);
                componentId = response.Data.Component.Id;
            }

            // Проверим, что нельзя создать компонент сверх лимита
            var request2 = new GetOrCreateComponentRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateComponentRequestData()
                {
                    SystemName = "Component." + Guid.NewGuid(),
                    TypeId = componentType
                }
            };
            var response2 = dispatcher.GetOrCreateComponent(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Удалим последний компонент
            var deleteRequest = new DeleteComponentRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteComponentRequestData()
                {
                    ComponentId = componentId
                }
            };
            var deleteResponse = dispatcher.DeleteComponent(deleteRequest);
            Assert.True(deleteResponse.Success);

            // Теперь можно добавить ещё один компонент
            response2 = dispatcher.GetOrCreateComponent(request2);
            Assert.True(response2.Success);
        }

        [Fact]
        public void ComponentTypesCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим лимит аккаунта
            const int componentTypesMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.ComponentTypesMax = limits.UsedInstant.ComponentTypesCount + componentTypesMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var componentTypeId = Guid.Empty;
            // Проверим, что можно создавать типы компонентов до лимита
            for (var i = 0; i < componentTypesMax; i++)
            {
                var request = new GetOrCreateComponentTypeRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateComponentTypeRequestData()
                    {
                        SystemName = "ComponentType." + Guid.NewGuid()
                    }
                };
                var response = dispatcher.GetOrCreateComponentType(request);
                response.Check();
                componentTypeId = response.Data.Id;
            }

            // Проверим, что нельзя создать тип компонента сверх лимита
            var request2 = new GetOrCreateComponentTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateComponentTypeRequestData()
                {
                    SystemName = "ComponentType." + Guid.NewGuid()
                }
            };
            var response2 = dispatcher.GetOrCreateComponentType(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Удалим последний тип компонента
            var deleteRequest = new DeleteComponentTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteComponentTypeRequestData()
                {
                    ComponentTypeId = componentTypeId
                }
            };
            var deleteResponse = dispatcher.DeleteComponentType(deleteRequest);
            Assert.True(deleteResponse.Success);

            // Теперь можно добавить ещё один тип компонента
            response2 = dispatcher.GetOrCreateComponentType(request2);
            Assert.True(response2.Success);

        }

        [Fact]
        public void UnitTestTypesCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим лимит аккаунта
            const int unitTestTypesMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.UnitTestTypesMax = limits.UsedInstant.UnitTestTypesCount + unitTestTypesMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var unitTestTypeId = Guid.Empty;
            // Проверим, что можно создавать типы проверок до лимита
            for (var i = 0; i < unitTestTypesMax; i++)
            {
                var request = new GetOrCreateUnitTestTypeRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateUnitTestTypeRequestData()
                    {
                        SystemName = "UnitTestType." + Guid.NewGuid()
                    }
                };
                var response = dispatcher.GetOrCreateUnitTestType(request);
                response.Check();
                unitTestTypeId = response.Data.Id;
            }

            // Проверим, что нельзя создать тип проверки сверх лимита
            var request2 = new GetOrCreateUnitTestTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateUnitTestTypeRequestData()
                {
                    SystemName = "UnitTestType." + Guid.NewGuid()
                }
            };
            var response2 = dispatcher.GetOrCreateUnitTestType(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Удалим последний тип проверки
            var deleteRequest = new DeleteUnitTestTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteUnitTestTypeRequestData()
                {
                    UnitTestTypeId = unitTestTypeId
                }
            };
            var deleteResponse = dispatcher.DeleteUnitTestType(deleteRequest);
            Assert.True(deleteResponse.Success);

            // Теперь можно добавить ещё один тип проверки
            response2 = dispatcher.GetOrCreateUnitTestType(request2);
            Assert.True(response2.Success);

        }

        [Fact]
        public void HttpChecksNoBannerCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateRandomComponentControl();

            // Установим лимит аккаунта
            const int httpUnitTestsMaxNoBanner = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.HttpChecksMaxNoBanner = httpUnitTestsMaxNoBanner;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Создадим http-проверки на 1 больше, чем лимит без баннера
            // Все проверяют сайт без баннера
            var httpCheckId = Guid.Empty;
            for (var i = 0; i < httpUnitTestsMaxNoBanner + 1; i++)
            {
                var request = new GetOrCreateUnitTestRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateUnitTestRequestData()
                    {
                        SystemName = "HttpCheck." + Guid.NewGuid(),
                        ComponentId = component.Info.Id,
                        UnitTestTypeId = SystemUnitTestTypes.HttpUnitTestType.Id
                    }
                };
                var response = dispatcher.GetOrCreateUnitTest(request);
                Assert.True(response.Success);
                httpCheckId = response.Data.Id;
                using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
                {
                    var unitTestRepository = accountContext.GetUnitTestRepository();
                    var unitTest = unitTestRepository.GetById(httpCheckId);
                    var rule = new HttpRequestUnitTestRule()
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = "Rule",
                        Method = HttpRequestMethod.Get,
                        Url = "http://yandex.ru"
                    };
                    unitTest.HttpRequestUnitTest.Rules.Add(rule);
                    accountContext.SaveChanges();
                }
            }

            // Активируем проверку
            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), new TimeService());
            processor.ProcessAccount(account.Id, httpCheckId);
            Assert.Null(processor.DbProcessor.FirstException);
            account.SaveAllCaches();

            // Количество включенных проверок должно быть равно лимиту без баннера
            // Лишняя должна быть выключена
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTests = unitTestRepository.QueryAll().Where(t => t.ComponentId == component.Info.Id);
                int count = unitTests.Count(t => t.Enable && t.HttpRequestUnitTest.HasBanner == false);
                Assert.Equal(httpUnitTestsMaxNoBanner, count);
                Assert.Equal(1, unitTests.Count(t => t.Enable == false && t.HttpRequestUnitTest.HasBanner == false));
                httpCheckId = unitTests.Single(x => x.Enable == false).Id;
            }

            // Перенастроим последнюю проверку на сайт с баннером
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.GetById(httpCheckId);
                var rule = unitTest.HttpRequestUnitTest.Rules.First();
                rule.Url = "http://fakesite.zidium.net";
                unitTest.Enable = true;
                unitTest.NextExecutionDate = DateTime.Now;
                unitTest.HttpRequestUnitTest.LastBannerCheck = null;
                accountContext.SaveChanges();
            }

            // Выполним её
            processor.ProcessAccount(account.Id, httpCheckId);
            Assert.Null(processor.DbProcessor.FirstException);

            // Все проверки выполнить заново
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTests = unitTestRepository.QueryAll().Where(t => t.ComponentId == component.Info.Id).ToArray();
                foreach (var unitTest in unitTests)
                {
                    unitTest.Enable = true;
                    unitTest.NextExecutionDate = DateTime.Now;
                    unitTest.HttpRequestUnitTest.LastBannerCheck = null;
                    accountContext.SaveChanges();
                }
            }

            // Выполним проверки 2-ой  раз
            processor.ProcessAccount(account.Id, httpCheckId);
            Assert.Null(processor.DbProcessor.FirstException);

            // Все проверки должны быть включены
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();

                var unitTests = unitTestRepository
                    .QueryAll()
                    .Where(t => t.ComponentId == component.Info.Id)
                    .ToList();

                int count = unitTests.Count(t => t.Enable);
                Assert.Equal(httpUnitTestsMaxNoBanner + 1, count);
                var successUnitTests = unitTests.Where(t => t.Enable && t.HttpRequestUnitTest.HasBanner).ToArray();
                Assert.Equal(1, successUnitTests.Length);
                var offlineUnitTests = unitTests.Where(t => t.Enable == false).ToArray();
                Assert.Equal(0, offlineUnitTests.Length);
            }
        }

        [Fact]
        public void UnitTestsCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();

            // Установим лимит аккаунта
            const int unitTestsMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.UnitTestsMax = limits.UsedInstant.UnitTestsCount + unitTestsMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var apiCheckId = Guid.Empty;
            // Проверим, что можно создавать проверки до лимита
            for (var i = 0; i < unitTestsMax; i++)
            {
                var unitTestTypeId = TestHelper.CreateTestUnitTestType(account.Id).Id;
                var request = new GetOrCreateUnitTestRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateUnitTestRequestData()
                    {
                        SystemName = "Check." + Guid.NewGuid(),
                        ComponentId = component.Id,
                        UnitTestTypeId = unitTestTypeId
                    }
                };
                var response = dispatcher.GetOrCreateUnitTest(request);
                Assert.True(response.Success);
                apiCheckId = response.Data.Id;
            }

            // Проверим, что нельзя создать проверку сверх лимита
            var unitTestTypeId2 = TestHelper.CreateTestUnitTestType(account.Id).Id;
            var request2 = new GetOrCreateUnitTestRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateUnitTestRequestData()
                {
                    SystemName = "Check." + Guid.NewGuid(),
                    ComponentId = component.Id,
                    UnitTestTypeId = unitTestTypeId2
                }
            };
            var response2 = dispatcher.GetOrCreateUnitTest(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Удалим последнюю проверку
            var deleteRequest = new DeleteUnitTestRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteUnitTestRequestData()
                {
                    UnitTestId = apiCheckId
                }
            };
            var deleteResponse = dispatcher.DeleteUnitTest(deleteRequest);
            Assert.True(deleteResponse.Success);

            // Теперь можно добавить ещё одну проверку
            response2 = dispatcher.GetOrCreateUnitTest(request2);
            Assert.True(response2.Success);

        }

        [Fact]
        public void UnitTestRequestsPerDayTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();

            // Установим лимит аккаунта
            const int unitTestsRequestsPerDay = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.UnitTestsRequestsPerDay = limits.UsedToday.UnitTestsRequests + unitTestsRequestsPerDay;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var unitTest = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            // Проверим, что можно отправлять результаты до лимита
            for (var i = 0; i < unitTestsRequestsPerDay; i++)
            {
                var request = new SendUnitTestResultRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new SendUnitTestResultRequestData()
                    {
                        UnitTestId = unitTest.Id,
                        Result = UnitTestResult.Success
                    }
                };
                var response = dispatcher.SendUnitTestResult(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя отправить результат сверх лимита
            var request2 = new SendUnitTestResultRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendUnitTestResultRequestData()
                {
                    UnitTestId = unitTest.Id,
                    Result = UnitTestResult.Success
                }
            };
            var response2 = dispatcher.SendUnitTestResult(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Проверим, что нельзя отправить результат другой проверки
            var unitTest2 = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            request2 = new SendUnitTestResultRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendUnitTestResultRequestData()
                {
                    UnitTestId = unitTest2.Id,
                    Result = UnitTestResult.Success
                }
            };
            response2 = dispatcher.SendUnitTestResult(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);
        }

        [Fact]
        public void LogSizePerDayTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();

            // Установим лимит аккаунта
            const int logSizePerDay = 6000;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.LogSizePerDay = limits.UsedToday.LogSize + logSizePerDay;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Подготовим данные размером 582 байт
            var data = new SendLogData()
            {
                ComponentId = component.Id,
                Message = new string('x', 150), // 300 байт
                Properties = new List<ExtentionPropertyDto>()
            };
            data.Properties.AddValue("Number", 999999); // 12 байт
            data.Properties.AddValue("Stack", new string('s', 94)); // 188 байт
            Assert.Equal(6022, data.GetSize());

            // Проверим, что можно отправлять логи до лимита размера
            Int64 size = 0;
            while (true)
            {
                var request = new SendLogRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = data
                };
                size += data.GetSize();

                if (size > logSizePerDay)
                    break;

                var response = dispatcher.SendLog(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя отправить лог сверх лимита размера
            var request2 = new SendLogRequest()
            {
                Token = account.GetCoreToken(),
                Data = data
            };
            var response2 = dispatcher.SendLog(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);
        }

        [Fact]
        public void EventRequestsPerDayTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Установим лимит аккаунта
            const int eventsRequestsPerDay = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.EventRequestsPerDay = limits.UsedToday.EventRequests + eventsRequestsPerDay;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Проверим, что можно отправлять события до лимита
            for (var i = 0; i < eventsRequestsPerDay; i++)
            {
                var request = new SendEventRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new SendEventData()
                    {
                        ComponentId = component.Id,
                        Category = EventCategory.ApplicationError,
                        TypeSystemName = eventType.SystemName,
                        Message = "Test"
                    }
                };
                var response = dispatcher.SendEvent(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя отправить событие сверх лимита
            var request2 = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Id,
                    Category = EventCategory.ComponentEvent,
                    TypeSystemName = eventType.SystemName,
                    Message = "Test"
                }
            };
            var response2 = dispatcher.SendEvent(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);
        }

        [Fact]
        public void MetricsCountTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();

            // Установим лимит аккаунта
            const int metricsMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.MetricsMax = limits.UsedInstant.MetricsCount + metricsMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Проверим, что можно создавать метрики до лимита
            for (var i = 0; i < metricsMax; i++)
            {
                var request = new SendMetricRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new SendMetricRequestData()
                    {
                        ComponentId = component.Id,
                        Name = Guid.NewGuid().ToString(),
                        Value = new Random().NextDouble(),
                        ActualIntervalSecs = 100
                    }
                };
                var response = dispatcher.SendMetric(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя создать метрику сверх лимита
            var request2 = new SendMetricRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendMetricRequestData()
                {
                    ComponentId = component.Id,
                    ActualIntervalSecs = 100,
                    Name = Guid.NewGuid().ToString(),
                    Value = new Random().NextDouble()
                }
            };
            var response2 = dispatcher.SendMetric(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);

            // Получим любую метрику
            Guid counterId;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var counterRepository = accountContext.GetMetricTypeRepository();
                var counter = counterRepository.QueryAll().First();
                counterId = counter.Id;
            }

            // Удалим эту метрику
            var deleteRequest = new DeleteMetricTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteMetricTypeRequestData()
                {
                    MetricTypeId = counterId
                }
            };
            var deleteResponse = dispatcher.DeleteMetricType(deleteRequest);
            Assert.True(deleteResponse.Success);

            // Теперь можно добавить ещё одну метрику
            response2 = dispatcher.SendMetric(request2);
            Assert.True(response2.Success);
        }

        [Fact]
        public void MetricRequestsPerDayTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();

            // Установим лимит аккаунта
            const int metricsRequestsPerDay = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.MetricRequestsPerDay = limits.UsedToday.MetricRequests + metricsRequestsPerDay;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Проверим, что можно отправлять данные по одному метрике до лимита
            var counterName = Guid.NewGuid().ToString();
            for (var i = 0; i < metricsRequestsPerDay; i++)
            {
                var request = new SendMetricRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new SendMetricRequestData()
                    {
                        ComponentId = component.Id,
                        Name = counterName,
                        Value = new Random().NextDouble()
                    }
                };
                var response = dispatcher.SendMetric(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя отправить данные метрики сверх лимита
            var request2 = new SendMetricRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendMetricRequestData()
                {
                    ComponentId = component.Id,
                    Name = counterName,
                    Value = new Random().NextDouble()
                }
            };
            var response2 = dispatcher.SendMetric(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);
        }

        [Fact]
        public void SmsPerDayTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим лимит аккаунта
            const int smsPerDay = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.SmsPerDay = limits.UsedToday.SmsCount + smsPerDay;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Проверим, что можно отправлять sms до лимита
            for (var i = 0; i < smsPerDay; i++)
            {
                var request = new SendSmsRequest()
                {
                    Token = account.GetCoreLocalToken(),
                    Data = new SendSmsRequestData()
                    {
                        Phone = "-",
                        Body = "-"
                    }
                };
                var response = dispatcher.SendSms(request);
                Assert.True(response.Success);
            }

            // Проверим, что нельзя отправить sms сверх лимита
            var request2 = new SendSmsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SendSmsRequestData()
                {
                    Phone = "-",
                    Body = "-"
                }
            };
            var response2 = dispatcher.SendSms(request2);
            Assert.False(response2.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, response2.Code);
        }

        [Fact]
        public void StorageSizeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();
            var unitTest = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            // Подготовим по одному запросу: проверка, событие, лог, метрика

            var unitTestResultRequest = new SendUnitTestResultRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendUnitTestResultRequestData()
                {
                    UnitTestId = unitTest.Id,
                    Result = UnitTestResult.Success,
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            unitTestResultRequest.Data.Properties.AddValue("Property", "Value");

            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Id,
                    Category = EventCategory.ApplicationError,
                    Importance = EventImportance.Alarm,
                    TypeSystemName = Guid.NewGuid().ToString(),
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            eventRequest.Data.Properties.AddValue("Property", "Value");

            var logRequest = new SendLogRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendLogData()
                {
                    ComponentId = component.Id,
                    Message = "Test",
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            logRequest.Data.Properties.AddValue("Property", "Value");

            var metricRequest = new SendMetricRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendMetricRequestData()
                {
                    ComponentId = component.Id,
                    Name = Guid.NewGuid().ToString(),
                    Value = new Random().NextDouble()
                }
            };

            // Установим лимит аккаунта равным сумме размеров запросов

            var storageSizeMax =
                unitTestResultRequest.Data.GetSize() +
                eventRequest.Data.GetSize() +
                logRequest.Data.GetSize() +
                metricRequest.Data.GetSize();

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.StorageSizeMax = limits.UsedOverall.Total.StorageSize + storageSizeMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            // Проверим, что запросы успешно выполняются

            var apiCheckResultResponse = dispatcher.SendUnitTestResult(unitTestResultRequest);
            Assert.True(apiCheckResultResponse.Success);

            var eventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(eventResponse.Success);

            var logResponse = dispatcher.SendLog(logRequest);
            Assert.True(logResponse.Success);

            var metricResponse = dispatcher.SendMetric(metricRequest);
            Assert.True(metricResponse.Success);

            // Получим лимиты аккаунта
            var limitsResponse = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            });
            var usedLimits = limitsResponse.Data;

            // Проверим, что использованный лимит хранилища правильный
            Assert.Equal(limits.Hard.StorageSizeMax, usedLimits.UsedToday.StorageSize);

            // Проверим, что больше выполнить действия нельзя

            apiCheckResultResponse = dispatcher.SendUnitTestResult(unitTestResultRequest);
            Assert.False(apiCheckResultResponse.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, apiCheckResultResponse.Code);

            eventResponse = dispatcher.SendEvent(eventRequest);
            Assert.False(eventResponse.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, eventResponse.Code);

            logResponse = dispatcher.SendLog(logRequest);
            Assert.False(logResponse.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, logResponse.Code);

            metricResponse = dispatcher.SendMetric(metricRequest);
            Assert.False(metricResponse.Success);
            Assert.Equal(Zidium.Api.ResponseCode.OverLimit, metricResponse.Code);
        }

        [Fact]
        public void MaxArchiveDaysTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим лимиты аккаунта

            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data.Hard;

            limits.StorageSizeMax = 111100;
            limits.LogSizePerDay = 100000;
            limits.LogMaxDays = 7;
            limits.EventsMaxDays = 6;
            limits.UnitTestsMaxDays = 5;
            limits.MetricsMaxDays = 4;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits,
                    Type = TariffLimitType.Soft
                }
            });

            var now = DateTime.Now.Date;
            var checker = new AccountLimitsChecker(account.Id, now);
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                // Потратим весь лимит хранилища
                checker.AddLogSizePerDay(accountContext, 100000);
                checker.AddEventsSizePerDay(accountContext, 10000);
                checker.AddUnitTestsSizePerDay(accountContext, 1000);
                checker.AddMetricsSizePerDay(accountContext, 100);

                // Проверим, что лимит хранилища достигнут
                var context = accountContext;
                Assert.ThrowsAny<OverLimitException>(() => checker.CheckStorageSize(context, 1, out _));

                // Получим использованные лимиты и проверим, что размер хранилища правильный
                var usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(111100, usedLimits.StorageSize);

                // Прошёл один день
                now = now.AddDays(1);
                checker.NowOverride = now;

                // Сохраним данные
                checker.SaveData(accountContext);

                // Проверим размер хранилища
                usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(111100, usedLimits.StorageSize);

                // Проверим размер хранилища через 4 дня
                now = now.AddDays(4);
                checker.NowOverride = now;
                usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(111000, usedLimits.StorageSize);
                checker.CheckStorageSize(accountContext, 100, out _);

                // Проверим размер хранилища через 5 дней
                now = now.AddDays(1);
                checker.NowOverride = now;
                usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(110000, usedLimits.StorageSize);
                checker.CheckStorageSize(accountContext, 1100, out _);

                // Проверим размер хранилища через 6 дней
                now = now.AddDays(1);
                checker.NowOverride = now;
                usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(100000, usedLimits.StorageSize);
                checker.CheckStorageSize(accountContext, 11100, out _);

                // Проверим размер хранилища через 7 дней
                now = now.AddDays(1);
                checker.NowOverride = now;
                usedLimits = checker.GetUsedOverallTariffLimit(accountContext, 30).Total;
                Assert.Equal(0, usedLimits.StorageSize);
                checker.CheckStorageSize(accountContext, 111100, out _);
            }
        }

        [Fact]
        public void ArchiveDataTest()
        {
            var account = TestHelper.GetTestAccount();
            var today = DateTime.Now.Date;

            // 30 дней назад
            var date30 = today.AddDays(-30);
            var checker = new AccountLimitsChecker(account.Id, date30);
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 100);
                checker.NowOverride = date30.AddDays(1);
                checker.SaveData(accountContext);
            }

            // 1 день назад
            var date1 = today.AddDays(-1);
            checker.NowOverride = date1;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 200);
                checker.NowOverride = date1.AddDays(1);
                checker.SaveData(accountContext);
            }

            // Проверим, что сегодня есть архив за 30 дней
            checker.NowOverride = today;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var archive = checker.GetUsedOverallTariffLimit(accountContext, 30).Archive;
                Assert.Equal(2, archive.Length);
                Assert.NotNull(archive.FirstOrDefault(t => t.Date == date30 && t.Info.LogSize == 100));
                Assert.NotNull(archive.FirstOrDefault(t => t.Date == date1 && t.Info.LogSize == 200));
            }

        }

        [Fact]
        public void LimitDatasPer5MinLoadTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var unitTest1 = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            var unitTest2 = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            // Выровняем сдвиг времени по границе N минут
            var now = DateTime.Now.Date.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            var checker = new AccountLimitsChecker(account.Id, now);

            // Отправим данные по первой проверке
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest1.Id);
            }

            // Отправим запись лога
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 100);
            }

            // Прошло N минут
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Отправим данные по второй проверке до лимита
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest2.Id);
            }

            // Прошло N минут
            checker.NowOverride = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);

            // Сохраним данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var count = checker.SaveData(accountContext);
                Assert.Equal(2, count);
            }

            // Перечитаем данные из базы
            var nowOverride = checker.NowOverride;
            checker = new AccountLimitsChecker(account.Id, nowOverride);

            AccountUsedLimitsTodayDataInfo usedLimits;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                usedLimits = checker.GetUsedTodayTariffLimit(accountContext);
            }

            // Проверим, что лимит по первой проверке не потерялся
            Assert.Equal(1, usedLimits.UnitTestsResults.Where(t => t.UnitTestId == unitTest1.Id).Select(t => t.ApiChecksResults).First());

            // Проверим, что лимит по второй проверке не потерялся
            Assert.Equal(1, usedLimits.UnitTestsResults.Where(t => t.UnitTestId == unitTest2.Id).Select(t => t.ApiChecksResults).First());

            // Проверим, что лимит по логу не потерялся
            Assert.Equal(100, usedLimits.LogSize);

        }

        [Fact]
        public void LimitDatasPer5MinSaveTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var unitTest1 = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            var unitTest2 = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            // Запомним количество строк в истории
            int limitDatasCount;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetLimitDataRepository();
                limitDatasCount = repository.QueryAll().Count();
            }

            // Выровняем сдвиг времени по границе N минут
            var now = DateTime.Now.Date.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            var checker = new AccountLimitsChecker(account.Id, now);

            // Добавим данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest1.Id);
                checker.AddUnitTestResultsPerDay(accountContext, unitTest2.Id);
                checker.AddEventsRequestsPerDay(accountContext);
                checker.AddEventsSizePerDay(accountContext, 100);
            }

            // Прошло N минут
            var oldNow = now;
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Вызовем сохранение
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var count = checker.SaveData(accountContext);
                Assert.Equal(1, count);
            }

            // Проверим, что появилась новая строка в истории
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetLimitDataRepository();
                var rows = repository.QueryAll();
                var limitDatasCount2 = rows.Count();
                Assert.True(limitDatasCount2 > limitDatasCount);

                // Получим последнюю строку
                var data = rows.OrderByDescending(t => t.BeginDate).FirstOrDefault();
                Assert.NotNull(data);

                // Проверим, как записались данные
                var beginDate = new DateTime(oldNow.Year, oldNow.Month, oldNow.Day, oldNow.Hour, (oldNow.Minute / AccountLimitsChecker.LimitDataTimeStep) * AccountLimitsChecker.LimitDataTimeStep, 0);
                Assert.Equal(beginDate, data.BeginDate);
                Assert.Equal(beginDate.AddMinutes(AccountLimitsChecker.LimitDataTimeStep), data.EndDate);
                Assert.True(data.UnitTestData.Any(t => t.UnitTestId == unitTest1.Id && t.ResultsCount == 1));
                Assert.True(data.UnitTestData.Any(t => t.UnitTestId == unitTest2.Id && t.ResultsCount == 1));
                Assert.Equal(1, data.EventsRequests);

                limitDatasCount = limitDatasCount2;
            }

            // Добавим ещё данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest2.Id);
                checker.AddEventsRequestsPerDay(accountContext);
                checker.AddEventsSizePerDay(accountContext, 100);
            }

            // Прошло N минут
            oldNow = now;
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Затребуем данные по лимитам
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.GetUsedTodayTariffLimit(accountContext);
            }

            // Вызовем сохранение
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var count = checker.SaveData(accountContext);
                Assert.Equal(1, count);
            }

            // Проверим, что появилась новая строка в истории
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetLimitDataRepository();
                var rows = repository.QueryAll();
                var limitDatasCount2 = rows.Count();
                Assert.True(limitDatasCount2 > limitDatasCount);

                // Получим последнюю строку
                var data = rows.OrderByDescending(t => t.BeginDate).FirstOrDefault();
                Assert.NotNull(data);

                // Проверим, как записались данные
                var beginDate = new DateTime(oldNow.Year, oldNow.Month, oldNow.Day, oldNow.Hour, (oldNow.Minute / AccountLimitsChecker.LimitDataTimeStep) * AccountLimitsChecker.LimitDataTimeStep, 0);
                Assert.Equal(beginDate, data.BeginDate);
                Assert.Equal(beginDate.AddMinutes(AccountLimitsChecker.LimitDataTimeStep), data.EndDate);
                Assert.False(data.UnitTestData.Any(t => t.UnitTestId == unitTest1.Id));
                Assert.True(data.UnitTestData.Any(t => t.UnitTestId == unitTest2.Id && t.ResultsCount == 1));
                Assert.Equal(1, data.EventsRequests);
                Assert.Equal(100, data.EventsSize);
            }

            // Перечитаем данные из базы
            checker = new AccountLimitsChecker(account.Id, now);

            // Проверим сохранение пустой записи
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var count = checker.SaveData(accountContext);
                Assert.Equal(1, count);
            }

            // Проверим сохранение сразу 2 записей
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest1.Id);
                checker.AddUnitTestResultsPerDay(accountContext, unitTest2.Id);
                checker.AddEventsRequestsPerDay(accountContext);
                checker.AddEventsSizePerDay(accountContext, 100);
            }

            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddUnitTestResultsPerDay(accountContext, unitTest1.Id);
                checker.AddUnitTestResultsPerDay(accountContext, unitTest2.Id);
                checker.AddEventsRequestsPerDay(accountContext);
                checker.AddEventsSizePerDay(accountContext, 100);
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var count = checker.SaveData(accountContext);
                Assert.Equal(2, count);
                count = checker.SaveData(accountContext);
                Assert.Equal(0, count);
            }
        }

        [Fact]
        public void CrossTodayBoundTest()
        {
            var account = TestHelper.GetTestAccount();

            // Установим время на конец дня (23:59)
            var now = DateTime.Now.Date.AddMinutes(-1);
            var checker = new AccountLimitsChecker(account.Id, now);

            // Отправим данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 100);
            }

            // Проверим, что использованный лимит за сегодня заполнен
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var usedLimits = checker.GetUsedTodayTariffLimit(accountContext);
                Assert.Equal(100, usedLimits.LogSize);
            }

            // Установим время на начало следующего дня (00:01)
            now = now.AddMinutes(2);
            checker.NowOverride = now;

            // Проверим, что использованный лимит за сегодня пустой, так как начался новый день
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var usedLimits = checker.GetUsedTodayTariffLimit(accountContext);
                Assert.Equal(0, usedLimits.LogSize);
            }
        }

        [Fact]
        public void LimitDatasPer1DayLoadTest()
        {
            var account = TestHelper.GetTestAccount();

            // Подготовим данные за вчера и сохраним их

            var today = DateTime.Now.Date;
            var now = today.AddMinutes(1);
            var checker = new AccountLimitsChecker(account.Id, now);

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 101);
                checker.AddEventsSizePerDay(accountContext, 102);
                checker.AddUnitTestsSizePerDay(accountContext, 103);
                checker.AddMetricsSizePerDay(accountContext, 104);
            }

            var yesterday = today;
            now = today.AddDays(1);
            checker.NowOverride = now;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 10);
                checker.AddEventsSizePerDay(accountContext, 20);
                checker.AddUnitTestsSizePerDay(accountContext, 30);
                checker.AddMetricsSizePerDay(accountContext, 40);
            }

            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.SaveData(accountContext);
            }

            // Перечитаем данные из базы
            checker = new AccountLimitsChecker(account.Id, now);

            // Получим использованные лимиты
            AccountUsedLimitsPerDayDataInfo usedOverallLimits;
            AccountUsedLimitsArchiveDataInfo[] usedArchiveLimits;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var limits = checker.GetUsedOverallTariffLimit(accountContext, 30);
                usedOverallLimits = limits.Total;
                usedArchiveLimits = limits.Archive;
            }

            // Проверим, что данные не потерялись
            Assert.Equal(111, usedOverallLimits.LogSize);
            Assert.Equal(122, usedOverallLimits.EventsSize);
            Assert.Equal(133, usedOverallLimits.UnitTestsSize);
            Assert.Equal(144, usedOverallLimits.MetricsSize);

            var yesterdayUsedLimits = usedArchiveLimits.FirstOrDefault(t => t.Date == yesterday);
            Assert.NotNull(yesterdayUsedLimits);
            Assert.Equal(101, yesterdayUsedLimits.Info.LogSize);
            Assert.Equal(102, yesterdayUsedLimits.Info.EventsSize);
            Assert.Equal(103, yesterdayUsedLimits.Info.UnitTestsSize);
            Assert.Equal(104, yesterdayUsedLimits.Info.MetricsSize);
        }

        [Fact]
        public void LimitDatasPer1DaySaveTest()
        {
            var account = TestHelper.GetTestAccount();

            // Установим время на начало дня
            var today = DateTime.Now.Date;
            var now = today.AddMinutes(1);
            var checker = new AccountLimitsChecker(account.Id, now);

            // Отправим данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 100);
            }

            // Установим время на конец дня
            now = today.AddDays(1).AddMinutes(-1);
            checker.NowOverride = now;

            // Отправим данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 200);
            }

            // Вызовем сохранение
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.SaveData(accountContext);
            }

            // Проверим, что записи за вчера пока нет
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var r = accountContext.GetLimitDataRepository();
                var data1Day = r.QueryAll().FirstOrDefault(t => t.Type == LimitDataType.Per1Day && t.BeginDate == today);
                Assert.Null(data1Day);
            }

            // Начался новый день
            now = today.AddDays(1);
            checker.NowOverride = now;

            // Отправим данные
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.AddLogSizePerDay(accountContext, 50);
            }

            // Вызовем сохранение
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                checker.SaveData(accountContext);
            }

            // Проверим, что запись за вчера появилась
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var r = accountContext.GetLimitDataRepository();
                var data1Day = r.QueryAll().FirstOrDefault(t => t.Type == LimitDataType.Per1Day && t.BeginDate == today);
                Assert.NotNull(data1Day);
                Assert.Equal(300, data1Day.LogSize);
            }
        }

        [Fact]
        public void GetAccountLimitsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Запомним начальные лимиты
            var response = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            });
            Assert.True(response.Success);
            var baseAccountLimits = response.Data;

            // Выполним по одному действию каждого типа
            var componentTypeResponse = dispatcher.GetOrCreateComponentType(new GetOrCreateComponentTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateComponentTypeRequestData()
                {
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(componentTypeResponse.Success);

            var componentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateComponentRequestData()
                {
                    TypeId = componentTypeResponse.Data.Id,
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(componentResponse.Success);

            var logRequest = new SendLogRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendLogData()
                {
                    ComponentId = componentResponse.Data.Component.Id,
                    Message = "Test",
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            logRequest.Data.Properties.AddValue("Property", "Value");
            var logResponse = dispatcher.SendLog(logRequest);
            Assert.True(logResponse.Success);

            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = componentResponse.Data.Component.Id,
                    Category = EventCategory.ApplicationError,
                    Importance = EventImportance.Alarm,
                    TypeSystemName = Guid.NewGuid().ToString(),
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            eventRequest.Data.Properties.AddValue("Property", "Value");
            var eventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(eventResponse.Success);

            var unitTypeResponse = dispatcher.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateUnitTestTypeRequestData()
                {
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(unitTypeResponse.Success);

            var apiCheckResponse = dispatcher.GetOrCreateUnitTest(new GetOrCreateUnitTestRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateUnitTestRequestData()
                {
                    ComponentId = componentResponse.Data.Component.Id,
                    SystemName = Guid.NewGuid().ToString(),
                    UnitTestTypeId = unitTypeResponse.Data.Id
                }
            });
            Assert.True(apiCheckResponse.Success);

            var unitTestResultRequest = new SendUnitTestResultRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendUnitTestResultRequestData()
                {
                    UnitTestId = apiCheckResponse.Data.Id,
                    Result = UnitTestResult.Success
                }
            };

            var apiCheckResultResponse = dispatcher.SendUnitTestResult(unitTestResultRequest);
            Assert.True(apiCheckResultResponse.Success);

            var countersRequest = new SendMetricRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendMetricRequestData()
                {
                    ComponentId = componentResponse.Data.Component.Id,
                    Name = Guid.NewGuid().ToString(),
                    Value = new Random().NextDouble()
                }
            };
            var countersResponse = dispatcher.SendMetric(countersRequest);
            Assert.True(countersResponse.Success);

            var sendSmsRequest = new SendSmsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SendSmsRequestData()
                {
                    Phone = "-",
                    Body = "-"
                }
            };
            var sendSmsResponse = dispatcher.SendSms(sendSmsRequest);
            Assert.True(sendSmsResponse.Success);

            // Заново получим лимиты аккаунта
            response = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            });
            Assert.True(response.Success);
            var accountLimits = response.Data;

            // Теперь все использованные лимиты должны быть заполнены
            Assert.Equal(baseAccountLimits.UsedInstant.UnitTestsCount + 1, accountLimits.UsedInstant.UnitTestsCount);
            Assert.Equal(baseAccountLimits.UsedInstant.UnitTestTypesCount + 1, accountLimits.UsedInstant.UnitTestTypesCount);
            Assert.Equal(baseAccountLimits.UsedInstant.ComponentsCount + 1, accountLimits.UsedInstant.ComponentsCount);
            Assert.Equal(baseAccountLimits.UsedInstant.ComponentTypesCount + 1, accountLimits.UsedInstant.ComponentTypesCount);
            Assert.Equal(baseAccountLimits.UsedToday.EventRequests + 1, accountLimits.UsedToday.EventRequests);
            Assert.Equal(baseAccountLimits.UsedToday.EventsSize + eventRequest.Data.GetSize(), accountLimits.UsedToday.EventsSize);
            Assert.Equal(baseAccountLimits.UsedToday.LogSize + logRequest.Data.GetSize(), accountLimits.UsedToday.LogSize);
            Assert.Equal(baseAccountLimits.UsedInstant.MetricsCount + 1, accountLimits.UsedInstant.MetricsCount);
            Assert.Equal(baseAccountLimits.UsedToday.MetricsSize + countersRequest.Data.GetSize(), accountLimits.UsedToday.MetricsSize);
            Assert.Equal(baseAccountLimits.UsedToday.UnitTestsRequests + 1, accountLimits.UsedToday.UnitTestsRequests);
            Assert.Equal(baseAccountLimits.UsedToday.UnitTestsSize + unitTestResultRequest.Data.GetSize(), accountLimits.UsedToday.UnitTestsSize);
            Assert.True(accountLimits.UsedToday.UnitTestsResults.Any(t => t.UnitTestId == apiCheckResponse.Data.Id && t.ApiChecksResults == 1));
            Assert.Equal(baseAccountLimits.UsedToday.SmsCount + 1, accountLimits.UsedToday.SmsCount);

            var totalSize = eventRequest.Data.GetSize() + logRequest.Data.GetSize() + countersRequest.Data.GetSize() + unitTestResultRequest.Data.GetSize();
            Assert.Equal(baseAccountLimits.UsedToday.StorageSize + totalSize, accountLimits.UsedToday.StorageSize);
        }

        [Fact]
        public void OverlimitSignalTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Установим soft-лимит аккаунта
            const int componentsMax = 2;
            var limits = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetAccountLimitsRequestData()
            }).Data;

            limits.Hard.ComponentsMax = limits.UsedInstant.ComponentsCount + componentsMax;

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = limits.Hard,
                    Type = TariffLimitType.Soft
                }
            });

            var componentType = TestHelper.CreateRandomComponentTypeId(account.Id);

            // Создадим компоненты до лимита
            for (var i = 0; i < componentsMax; i++)
            {
                var request = new GetOrCreateComponentRequest()
                {
                    Token = account.GetCoreToken(),
                    Data = new GetOrCreateComponentRequestData()
                    {
                        SystemName = "Component." + Guid.NewGuid(),
                        TypeId = componentType
                    }
                };
                var response = dispatcher.GetOrCreateComponent(request);
                Assert.True(response.Success);
            }

            // Проверим, что у аккаунта нет признака превышения лимита
            var accountInfo = ConfigDbServicesHelper.GetAccountService().GetOneById(account.Id);
            Assert.Null(accountInfo.LastOverLimitDate);

            // Создадим ещё компонент - превысим Soft-лимит
            var request2 = new GetOrCreateComponentRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateComponentRequestData()
                {
                    SystemName = "Component." + Guid.NewGuid(),
                    TypeId = componentType
                }
            };
            var response2 = dispatcher.GetOrCreateComponent(request2);
            Assert.True(response2.Success);
        }

        public TariffLimitTests()
        {
            // Перед каждым тестом очистим историю изменения лимитов
            var account = TestHelper.GetTestAccount();

            using (var accountDbContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                foreach (var entity in accountDbContext.LimitDatasForUnitTests.ToArray())
                    accountDbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

                foreach (var entity in accountDbContext.LimitDatas.ToArray())
                    accountDbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

                accountDbContext.SaveChanges();
            }

            // Сбросим признак превышения лимита
            var dispatcher = DispatcherHelper.GetDispatcherClient();
            dispatcher.UpdateAccount(new UpdateAccountRequestData()
            {
                Id = account.Id,
                ClearLastOverLimitDate = true,
                ClearOverlimitEmailDate = true
            });
        }

        public void Dispose()
        {
            // После каждого теста вернём бесконечные лимиты
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = SystemTariffLimits.BaseUnlimited,
                    Type = TariffLimitType.Hard
                }
            });
            dispatcher.SetAccountLimits(new SetAccountLimitsRequest()
            {
                Token = account.GetCoreLocalToken(),
                Data = new SetAccountLimitsRequestData()
                {
                    Limits = SystemTariffLimits.BaseUnlimited,
                    Type = TariffLimitType.Soft
                }
            });
        }
    }
}
