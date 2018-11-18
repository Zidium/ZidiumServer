using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class UnitTestResult2Model
    {
        public UnitTest UnitTest { get; private set; }        
        
        public Event[] Statuses { get; set; }

        public void Init(UnitTest unitTest, Event[] statuses)
        {
            UnitTest = unitTest;
            Statuses = statuses;
        }

        public string PageTitle
        {
            get
            {
                UnitTestType type = UnitTest.Type;
                if (type.IsSystem == false)
                {
                    return "Пользовательская проверка: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.HttpUnitTestType.Id)
                {
                    return "Проверка HTTP: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.SqlTestType.Id)
                {
                    return "Проверка SQL: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.PingTestType.Id)
                {
                    return "Проверка ping: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.SslTestType.Id)
                {
                    return "Проверка SSL сертификата: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.DomainNameTestType.Id)
                {
                    return "Проверка домена: " + UnitTest.DisplayName;
                }
                return "Проверка: " + UnitTest.DisplayName;
            }
        }
    }
}