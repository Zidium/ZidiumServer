namespace Zidium.Api.Dto
{
    /// <summary>
    /// Информация для проверки прав доступа
    /// </summary>
    public class AccessTokenDto
    {
        /// <summary>
        /// Секретный ключ для доступа к Api
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Имя программы, которая исползует Api. Необязательный параметр.
        /// </summary>
        public string Program { get; set; }
    }
}
