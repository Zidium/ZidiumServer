using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка аккаунта
    /// </summary>
    public class AccountSettingForRead
    {
        public AccountSettingForRead(
            Guid id, 
            string name, 
            string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public Guid Id { get;}

        public string Name { get;}

        public string Value { get;}

        public AccountSettingForUpdate GetForUpdate()
        {
            return new AccountSettingForUpdate(Id);
        }
        
    }
}
