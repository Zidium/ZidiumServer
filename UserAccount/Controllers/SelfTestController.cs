using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Zidium.Core.ConfigDb;

namespace Zidium.UserAccount.Controllers
{
    public class SelfTestController : Controller
    {
        private void ConfigDbContextTest()
        {
            ConfigDbServicesHelper.GetAccountService().GetSystemAccount();
        }

        private void ComponentFakeTest()
        {
            var component = MvcApplication.ComponentControl;
            if (component.IsFake())
            {
                throw new Exception("component.IsFake()");
            }
        }

        private void EchoTest()
        {
            var component = MvcApplication.ComponentControl;
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
            tests.Add("ConfigDbContextTest", ConfigDbContextTest);
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