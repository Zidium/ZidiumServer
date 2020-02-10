using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ChecksController : ContextController
    {
        public ActionResult Results()
        {
            var r = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTests = r.QueryAll()
                .Include("Bulb")
                .Include("Type")
                .Include("Component");

            var model = new ResultsModel()
            {
                Tests = unitTests.ToList()
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
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTest = repository.GetById(id);

            if (unitTest.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id)
            {
                return RedirectToAction("Show", "HttpRequestCheck", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.PingTestType.Id)
            {
                return RedirectToAction("Show", "PingChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.SqlTestType.Id)
            {
                return RedirectToAction("Show", "SqlChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.DomainNameTestType.Id)
            {
                return RedirectToAction("Show", "DomainNamePaymentPeriodChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.SslTestType.Id)
            {
                return RedirectToAction("Show", "SslCertificateExpirationDateChecks", new { id });
            }

            return RedirectToAction("ResultDetails", "UnitTests", new { id });
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTest = repository.GetById(id);            

            var editAction = unitTest.SimpleMode ? "EditSimple" : "Edit";

            if (unitTest.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id)
            {
                return RedirectToAction(editAction, "HttpRequestCheck", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.PingTestType.Id)
            {
                return RedirectToAction(editAction, "PingChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.TcpPortTestType.Id)
            {
                return RedirectToAction(editAction, "TcpPortChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.SqlTestType.Id)
            {
                return RedirectToAction(editAction, "SqlChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.DomainNameTestType.Id)
            {
                return RedirectToAction(editAction, "DomainNamePaymentPeriodChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.SslTestType.Id)
            {
                return RedirectToAction(editAction, "SslCertificateExpirationDateChecks", new { id });
            }

            if (unitTest.TypeId == SystemUnitTestTypes.VirusTotalTestType.Id)
            {
                return RedirectToAction(editAction, "VirusTotal", new { id });
            }

            return RedirectToAction("Edit", "UnitTests", new { id });
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTest = repository.GetById(id);

            if (unitTest == null)
            {
                throw new UserFriendlyException("Не удалось найти проверку с Id = " + id);
            }
            unitTest.IsDeleted = true;
            CurrentAccountDbContext.SaveChanges();
            return RedirectToAction("Index", "UnitTests", new {unitTestTypeId = unitTest.TypeId});
        }
    }
}