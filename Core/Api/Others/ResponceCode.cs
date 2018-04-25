//namespace Zidium.Core.Api
//{
//    public static class ResponseCode
//    {
//        /// <summary>
//        /// Метод веб-сервиса выполнен успешно (без ошибок)
//        /// </summary>
//        public static readonly int Success = 10;

//        /// <summary>
//        /// Тело запроса имеет неверный формат
//        /// </summary>
//        public static readonly int RequestFormatError = 20;

//        /// <summary>
//        /// Неверный AccountId
//        /// </summary>
//        public static readonly int InvalidAccountId = 30;

//        /// <summary>
//        /// Неверный секретный ключ
//        /// </summary>
//        public static readonly int InvalidSecretKey = 31;

//        /// <summary>
//        /// Вызов не является локальным
//        /// </summary>
//        public static readonly int NonLocalRequest = 32;

//        /// <summary>
//        /// Аккаунт заблокирован
//        /// </summary>
//        public static readonly int AccountBlocked = 40;

//        /// <summary>
//        /// Операция заблокирована
//        /// </summary>
//        public static readonly int OperationBlocked = 50;

//        /// <summary>
//        /// Ошибка на стороне сервера мониторинга
//        /// </summary>
//        public static readonly int ServerError = 80;

//        /// <summary>
//        /// Сервер мониторинга перегружен. Обработка запросов приостановлена.
//        /// </summary>
//        public static readonly int ServerIsTooBusy = 90;

//        /// <summary>
//        /// Неизвестный идентификатор компонента
//        /// </summary>
//        public static readonly int UnknownComponentId = 100;

//        /// <summary>
//        /// Неизвестный идентификатор типа компонента
//        /// </summary>
//        public static readonly int UnknownComponentTypeId = 101;

//        /// <summary>
//        /// Ошибка на стороне клиента (Например, не работает интернет)
//        /// </summary>
//        public static readonly int ClientError = 110;

//        /// <summary>
//        /// Ошибка обработки ответа в DispatcherProxy
//        /// </summary>
//        public static readonly int DispatcherProxyClientError = 111;

//        /// <summary>
//        /// Неверный код HTTP ответа (должен быть 200 = ОК)
//        /// </summary>
//        public static readonly int InvalidResponseCode = 112;

//        /// <summary>
//        /// Не удалось зарегистрировать компонента (уже существует)
//        /// </summary>
//        public static readonly int RegisteredComponentAlreadyExists = 120;

//        /// <summary>
//        /// Превышен лимит
//        /// </summary>
//        public static readonly int OverLimit = 130;

//        /// <summary>
//        /// Отсутствует обязательный параметр в запроса
//        /// </summary>
//        public static readonly int RequiredParameterError = 140;

//        /// <summary>
//        /// Старая дата события. 
//        /// Используется когда отправляет событие с датой меньше, чем дата начала последнего события
//        /// </summary>
//        public static readonly int OldEventData = 150;

//        /// <summary>
//        /// Отправлено событие из будущего (startDate > now.AddMinutes(5))
//        /// </summary>
//        public static readonly int FutureEvent = 151;

//        /// <summary>
//        /// Неизвестное системное имя юнит-теста
//        /// </summary>
//        public static readonly int UnknownUnitTestTypeSystemName = 160;

//        /// <summary>
//        /// У данного компонента нет юнит-теста заданного типа
//        /// </summary>
//        public static readonly int ComponentDoesNotHaveUnitTestType = 163;

//        /// <summary>
//        /// Юнит-тест задним числом не поддерживается
//        /// </summary>
//        public static readonly int OldUnitTestDate = 165;

//        /// <summary>
//        /// Не указан интервал актуальности юнит-теста
//        /// </summary>
//        public static readonly int UnitTestResultActualIntervalIsEmpty = 166;

//        /// <summary>
//        /// Неизвестное действие (метод) веб-сервиса
//        /// </summary>
//        public static readonly int UnknownAction = 200;

//        public static readonly int ObjectDisabled = 210;
//    }
//}
