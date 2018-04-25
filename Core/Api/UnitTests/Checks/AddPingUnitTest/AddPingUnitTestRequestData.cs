namespace Zidium.Core.Api
{
    public class AddPingUnitTestRequestData : AddCheckBaseRequestData
    {
        /// <summary>
        /// Хост
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs { get; set; }

        /// <summary>
        /// Количество попыток
        /// </summary>
        public int Attemps { get; set; }

    }
}
