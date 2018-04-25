namespace Zidium.Api
{
    /// <summary>
    /// Информация для проверки прав доступа
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// Секретный ключ аккаунта для доступа к API
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Имя программы, которая исползует АПИ. Необязательный параметр.
        /// </summary>
        public string Program { get; set; }
    }
}
