using Zidium.Api.Dto;
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

        public static long GetSize(this SendUnitTestResultRequestDataDto data)
        {
            // Каждый результат проверки создаёт событие категории UnitTestResult
            // Поэтому считаем как для события
            long size = 0;
            if (data.Message != null)
            {
                size += data.Message.Length * 2;
            }
            if (data.Properties != null)
            {
                foreach (var property in data.Properties)
                {
                    if (property != null)
                    {
                        size += property.GetSize();
                    }
                }
            }
            return size;
        }

    }
}
