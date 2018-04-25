namespace Zidium.Api
{
    /// <summary>
    /// 1 - 99 = ошибки на стороне клиента
    /// 100 - 199 = ошибки на стороне веб-сервиса АПИ
    /// 200 - 299 = ошибки на стороне веб-сервиса диспетчера
    /// 1000 -  9999 = ошибки в диспетчере
    /// </summary>
    public static class ResponseCode
    {
        /// <summary>
        /// Метод веб-сервиса выполнен успешно (без ошибок)
        /// </summary>
        public static readonly int Success = 10;

        /// <summary>
        /// Веб-сервис временно недоступен, вместо него отвечает заглушка
        /// </summary>
        public static readonly int Offine = 15;

        /// <summary>
        /// Ошибка на стороне клиента (Например, не работает интернет)
        /// </summary>
        public static readonly int ClientError = 20;

        /// <summary>
        /// Не указан AccountName
        /// </summary>
        public static readonly int EmptyAccountName = 26;

        /// <summary>
        /// Неверный формат имени аккаунта
        /// </summary>
        public static readonly int AccountNameFormatError = 27;

        /// <summary>
        /// Не указан SecretKey
        /// </summary>
        public static readonly int EmptySecretKey = 28;

        /// <summary>
        /// Неверный формат адреса Api
        /// </summary>
        public static readonly int ApiUrlFormatError = 29;

        /// <summary>
        /// Клиент не смог разобрать ответ
        /// </summary>
        public static readonly int ResponseParseError = 30;

        /// <summary>
        /// Неверный код HTTP ответа (должен быть 200 = ОК)
        /// </summary>
        public static readonly int InvalidHttpResponseCode = 40;

        /// <summary>
        /// Неизвестное действие (метод) веб-сервиса
        /// </summary>
        public static readonly int UnknownAction = 100;

        /// <summary>
        /// Веб-сервис АПИ не смог разобрать запрос
        /// </summary>
        public static readonly int RequestParseError = 101;

        /// <summary>
        /// Веб-сервис АПИ не смог обработать запрос
        /// </summary>
        public static readonly int WebServiceError = 110;

        /// <summary>
        /// Ошибка на стороне сервера мониторинга
        /// </summary>
        public static readonly int ServerError = 1000;

        /// <summary>
        /// Ошибка на стороне сервера мониторинга
        /// </summary>
        public static readonly int ServerOffline = 1001;
        
        /// <summary>
        /// Неверный секретный ключ
        /// </summary>
        public static readonly int InvalidSecretKey = 1002;

        /// <summary>
        /// Аккаунт заблокирован
        /// </summary>
        public static readonly int AccountBlocked = 1003;

        /// <summary>
        /// Операция заблокирована
        /// </summary>
        public static readonly int OperationBlocked = 1004;

        /// <summary>
        /// Объект выключен
        /// </summary>
        public static readonly int ObjectDisabled = 1005;

        /// <summary>
        /// Неверный AccountName
        /// </summary>
        public static readonly int InvalidAccountName = 1006;
        
        /// <summary>
        /// Неизвестный идентификатор компонента
        /// </summary>
        public static readonly int UnknownComponentId = 1100;

        /// <summary>
        /// Неизвестный идентификатор типа компонента
        /// </summary>
        public static readonly int UnknownComponentTypeId = 1101;

        /// <summary>
        /// Неизвестный идентификатор проверки
        /// </summary>
        public static readonly int UnknownUnitTestId = 1102;

        /// <summary>
        /// Неизвестный идентификатор события
        /// </summary>
        public static readonly int UnknownEventId = 1103;

        /// <summary>
        /// Превышен лимит
        /// </summary>
        public static readonly int OverLimit = 1130;

        /// <summary>
        /// Вызов не является локальным
        /// </summary>
        public static readonly int NonLocalRequest = 1132;

        /// <summary>
        /// Отсутствует обязательный параметр в запроса
        /// </summary>
        public static readonly int RequiredParameterError = 1140;

        /// <summary>
        /// Неверный параметр в запроса
        /// </summary>
        public static readonly int ParameterError = 1141;

        /// <summary>
        /// Нельзя изменять системный объект
        /// </summary>
        public static readonly int CantUpdateSystemObject = 1142;

        /// <summary>
        /// Старая дата события. 
        /// Используется когда отправляет событие с датой меньше, чем дата начала последнего события
        /// </summary>
        public static readonly int OldEventData = 1150;

        /// <summary>
        /// Отправлено событие из будущего (startDate > now.AddMinutes(5))
        /// </summary>
        public static readonly int FutureEvent = 1151;

        /// <summary>
        /// Сервер мониторинга перегружен. Обработка запросов приостановлена.
        /// </summary>
        public static readonly int ServerIsTooBusy = 1190;

        /// <summary>
        /// Разнообразные ошибки - слишком общая ситуация для отдельного кода
        /// </summary>
        public static readonly int CommonError = int.MaxValue;
    }
}
