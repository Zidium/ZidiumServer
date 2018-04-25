namespace Zidium.Core.AccountsDb
{
    // Тип контакта пользователя
    public enum UserContactType
    {
        Email = 1,

        MobilePhone = 2,

        // Убраны те, которые мы не умеем отправлять
        /*
        Skype = 3,

        Icq = 4,
        */

        Http = 5
    }
}
