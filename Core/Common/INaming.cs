namespace Zidium.Core.Common
{
    /// <summary>
    /// Наименование объекта
    /// </summary>
    public interface INaming
    {
        /// <summary>
        /// Именительный падеж
        /// </summary>
        /// <returns></returns>
        string Nominative();

        /// <summary>
        /// Объект не найден
        /// </summary>
        /// <returns></returns>
        string NotFound();

        /// <summary>
        /// Дательный падеж
        /// </summary>
        /// <returns></returns>
        string Dative();
    }
}
