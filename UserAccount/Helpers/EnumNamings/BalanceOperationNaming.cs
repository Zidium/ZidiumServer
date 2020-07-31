using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
{
    public class BalanceOperationNaming : IEnumNaming<BalanceOperation>
    {
        public string Name(BalanceOperation value)
        {
            if (value == BalanceOperation.Income)
                return "Пополнение";

            if (value == BalanceOperation.Expense)
                return "Списание";

            return value.ToString();
        }
    }
}