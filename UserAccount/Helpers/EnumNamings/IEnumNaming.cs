namespace Zidium.UserAccount.Helpers
{
    public interface IEnumNaming<in T>
    {
        string Name(T value);
    }
}
