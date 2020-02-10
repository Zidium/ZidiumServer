namespace Zidium.Core.AccountsDb
{
    public interface IAccountSettingRepository
    {
        string GetValue(string name);

        void SetValue(string name, string value);
    }
}
