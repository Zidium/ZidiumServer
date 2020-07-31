using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка аккаунта
    /// </summary>
    public class DbAccountSetting
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
