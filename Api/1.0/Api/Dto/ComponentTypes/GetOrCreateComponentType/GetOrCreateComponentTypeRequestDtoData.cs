namespace Zidium.Api.Dto
{
    /// <summary>
    /// Сообщение на регистрацию пользовательского типа компонента
    /// </summary>
    public class GetOrCreateComponentTypeRequestDtoData
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
