namespace Zidium.Core.Api
{
    /// <summary>
    /// Сообщение на регистрацию пользовательского типа компонента
    /// </summary>
    public class GetOrCreateComponentTypeRequestData
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
