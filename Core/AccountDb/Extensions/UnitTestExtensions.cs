using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class UnitTestExtensions
    {
        /// <summary>
        /// Проверка и компонент включены
        /// </summary>
        public static bool CanProcess(this UnitTestForRead unittest)
        {
            return unittest.Enable && unittest.ParentEnable;
        }

        /// <summary>
        /// Проверяет что тип проверки системный
        /// </summary>
        public static bool IsSystemType(this UnitTestForRead unittest)
        {
            return SystemUnitTestType.IsSystem(unittest.TypeId);
        }

    }
}
