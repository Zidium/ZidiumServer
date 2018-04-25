using System;
using Zidium.Api.XmlConfig;

namespace Zidium.Api
{
    public interface IClient
    {
        #region Разное

        ResponseInfo LastResponse { get; }
        
        string ApiVersion { get; }
        
        IApiService ApiService { get; }

        IApiService SetApiService(IApiService service);

        bool Disable { get; set; }

        bool CanSendData { get; }

        /// <summary>
        /// Данные для доступа
        /// </summary>
        AccessToken AccessToken { get; }

        IEventManager EventManager { get; set; }

        Config Config { get; set; }

        bool CheckConnection();
        
        IExceptionRender ExceptionRender { get; set; }

        IEventPreparer EventPreparer { get; set; }

        void Flush();

        #endregion

        #region Время

        void CalculateServerTimeDifference();

        /// <summary>
        /// Разница между серверным временем и локальным временем.
        /// Если разница 2 сек = значит на сервере на 2 секунды больше.
        /// </summary>
        TimeSpan? TimeDifference { get; set; }

        /// <summary>
        /// Конвертирует дату и время с учетом разницы времени сервера и клиента. 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        DateTime ToServerTime(DateTime date);

        #endregion

        #region Типы компонентов
        
        IComponentTypeControl GetOrCreateComponentTypeControl(GetOrCreateComponentTypeData data);

        IComponentTypeControl GetOrCreateComponentTypeControl(string systemName);

        IComponentTypeControl GetFolderComponentTypeControl();

        #endregion

        #region Контролы компонентов
        
        IComponentControl GetRootComponentControl();

        IComponentControl GetDefaultComponentControl();

        IComponentControl GetComponentControl(Guid id);

        #endregion

        #region Типы проверок
        
        IUnitTestTypeControl GetOrCreateUnitTestTypeControl(GetOrCreateUnitTestTypeData data);

        IUnitTestTypeControl GetOrCreateUnitTestTypeControl(string systemName);

        IUnitTestTypeControl GetOrCreateUnitTestTypeControl(string systemName, string displayName);

        #endregion

        #region Лог

        IWebLogManager WebLogManager { get; set; }

        #endregion
    }
}
