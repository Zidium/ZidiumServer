namespace Zidium.Core.AccountsDb
{
    public enum UpdateStatusResultCode
    {
        /// <summary>
        /// Событие не повлияло на статус
        /// </summary>
        Canceled = 0,

        /// <summary>
        /// Статус продлен, но не изменился
        /// </summary>
        Prolonged = 1,

        /// <summary>
        /// Статус изменился
        /// </summary>
        Changed = 2
    }
}
