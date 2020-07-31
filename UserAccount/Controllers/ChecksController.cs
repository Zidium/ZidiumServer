using System;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ChecksController : BaseController
    {
        public ActionResult Results()
        {
            var unitTests = GetStorage().Gui.GetChecksResults();

            var model = new ResultsModel()
            {
                Tests = unitTests
            };

            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add()
        {
            var model = new AddModel();
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            if (unitTest.TypeId == SystemUnitTestType.HttpUnitTestType.Id)
            {
                return RedirectToAction("Show", "HttpRequestCheck", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.PingTestType.Id)
            {
                return RedirectToAction("Show", "PingChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.SqlTestType.Id)
            {
                return RedirectToAction("Show", "SqlChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.DomainNameTestType.Id)
            {
                return RedirectToAction("Show", "DomainNamePaymentPeriodChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.SslTestType.Id)
            {
                return RedirectToAction("Show", "SslCertificateExpirationDateChecks", new { id });
            }

            return RedirectToAction("ResultDetails", "UnitTests", new { id });
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            var editAction = unitTest.SimpleMode ? "EditSimple" : "Edit";

            if (unitTest.TypeId == SystemUnitTestType.HttpUnitTestType.Id)
            {
                return RedirectToAction(editAction, "HttpRequestCheck", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.PingTestType.Id)
            {
                return RedirectToAction(editAction, "PingChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.TcpPortTestType.Id)
            {
                return RedirectToAction(editAction, "TcpPortChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.SqlTestType.Id)
            {
                return RedirectToAction(editAction, "SqlChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.DomainNameTestType.Id)
            {
                return RedirectToAction(editAction, "DomainNamePaymentPeriodChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.SslTestType.Id)
            {
                return RedirectToAction(editAction, "SslCertificateExpirationDateChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestType.VirusTotalTestType.Id)
            {
                return RedirectToAction(editAction, "VirusTotal", new { id });
            }

            return RedirectToAction("Edit", "UnitTests", new { id });
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var client = GetDispatcherClient();
            client.DeleteUnitTest(CurrentUser.AccountId, id).Check();
            return RedirectToAction("Index", "UnitTests", new {unitTestTypeId = unitTest.TypeId});
        }
    }
}