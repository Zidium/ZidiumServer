using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api;

namespace Zidium.UserAccount.Controllers
{
    public class SelfTestController : BaseController
    {
        public SelfTestController(IComponentControl componentControl, ILogger<SelfTestController> logger) : base(logger)
        {
            _componentControl = componentControl;
        }

        private readonly IComponentControl _componentControl;

        private void StorageTest()
        {
            GetStorage().Check();
        }

        private void ComponentFakeTest()
        {
            var component = _componentControl;
            if (component.IsFake())
            {
                throw new Exception("component.IsFake()");
            }
        }

        private void EchoTest()
        {
            var component = _componentControl;
            var response = component.Client.ApiService.GetEcho("123");
            if (!response.Success)
            {
                throw new Exception("Ошибка команды эхо: " + response.ErrorMessage);
            }
        }

        public ActionResult Index()
        {
            // Здесь можно реализовать какие угодно проверки, чем проверок больше, тем лучше.
            var tests = new Dictionary<string, Action>();
            tests.Add("ConfigDbContextTest", StorageTest);
            tests.Add("ComponentFakeTest", ComponentFakeTest);
            tests.Add("EchoTest", EchoTest);

            bool success = true;
            var log = new StringBuilder();
            foreach (var testPair in tests)
            {
                var testAction = testPair.Value;
                var testName = testPair.Key;
                log.AppendLine("----------------------------------");
                try
                {
                    testAction();
                    log.AppendLine(testName + ": success");
                }
                catch (Exception exception)
                {
                    success = false;
                    log.AppendLine(testName + ": error : " + exception.Message);
                }
            }
            var response = success ? "###### SUCCESS ######" : "###### ERROR ######";
            response = response + Environment.NewLine + log.ToString();
            return Content(response, "text/plain");
        }
    }
}