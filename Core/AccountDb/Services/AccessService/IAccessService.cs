namespace Zidium.Core.AccountDb
{
    /// <summary>
    /// Сервис проверки доступа
    /// </summary>
    internal interface IAccessService
    {
        /// <summary>
        /// Есть доступ к общему Api?
        /// </summary>
        public bool HasApiAccess(string key);

        /// <summary>
        /// Есть доступ к системному Api?
        /// </summary>
        public bool HasSystemAccess(string key);
    }
}
