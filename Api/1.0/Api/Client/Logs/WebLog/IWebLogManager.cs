namespace Zidium.Api
{
    /// <summary>
    /// Менеджер веб-лога
    /// </summary>
    public interface IWebLogManager: ILogManager
    {
        /// <summary>
        /// Добавляет компонент в список компонентов для периодической перезагрузки веб-конфига
        /// </summary>
        /// <param name="componentControl"></param>
        void BeginReloadConfig(IComponentControl componentControl);

        /// <summary>
        /// Удаляет компонент из списка компонентов для периодической перезагрузки компонентов
        /// </summary>
        /// <param name="componentControl"></param>
        void EndReloadConfig(IComponentControl componentControl);

        /// <summary>
        /// Делегат для добавления новой записи лога
        /// </summary>
        event AddLogMessageDelegate OnAddLogMessage;
    }
}
