using System;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class UnitTestResult2Model
    {
        public UnitTestForRead UnitTest { get; private set; }        

        public UnitTestTypeForRead UnitTestType { get; set; }
        
        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs;

        public ComponentForRead Component;

        public DateTime Now;

        public BulbForRead UnitTestBulb;

        public string LastRunErrorMessage;

        // TODO Move to controller
        public void Init(UnitTestForRead unitTest)
        {
            UnitTest = unitTest;
        }

        // TODO Move to controller
        public string PageTitle
        {
            get
            {
                if (!UnitTestType.IsSystem)
                {
                    return "Пользовательская проверка: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.HttpUnitTestType.Id)
                {
                    return "Проверка HTTP: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.SqlTestType.Id)
                {
                    return "Проверка SQL: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.PingTestType.Id)
                {
                    return "Проверка ping: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.SslTestType.Id)
                {
                    return "Проверка SSL сертификата: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.DomainNameTestType.Id)
                {
                    return "Проверка домена: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.TcpPortTestType.Id)
                {
                    return "Проверка TCP-порта: " + UnitTest.DisplayName;
                }
                if (UnitTestType.Id == SystemUnitTestType.VirusTotalTestType.Id)
                {
                    return "Проверка Virus Total: " + UnitTest.DisplayName;
                }
                return "Проверка: " + UnitTest.DisplayName;
            }
        }
    }
}