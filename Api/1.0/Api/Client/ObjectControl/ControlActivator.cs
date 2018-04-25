using System;

namespace Zidium.Api
{
    /// <summary>
    /// 1. Любой IControl создается через IClient
    //  2. IClient гарантирует только 1 экземпляр IControl для 1 объекта (по полному системному имени)
    //  3. IClient всегда возвращает ControlWrapper (при этом запрос к веб-сервису НЕ делает = отложенная загрузка)
    //  4. ControlWrapper вначале создает ControlOffline, но при первом обращении заменяет его на ControlOnline
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    public class ControlActivator<TControl>
        where TControl : class, IObjectControl
    {
        protected TControl Control { get; set; }

        public ControlActivator(Func<TControl> getOnlineControlOrNull, Func<TControl> getOfflineControl)
        {
            if (getOnlineControlOrNull == null)
            {
                throw new ArgumentNullException("getOnlineControlOrNull");
            }
            if (getOfflineControl == null)
            {
                throw new ArgumentNullException("getOfflineControl");
            }
            GetOnlineControlOrNull = getOnlineControlOrNull;
            GetOfflineControl = getOfflineControl;
            Control = getOfflineControl();
            if (Control == null)
            {
                throw new Exception("getOfflineControl() is NULL");
            }
        }

        protected Func<TControl> GetOnlineControlOrNull { get; set; }

        protected Func<TControl> GetOfflineControl { get; set; }

        public TControl GetControl()
        {
            try
            {
                if (Control.IsFake() == false)
                {
                    return Control;
                }
                if (Control.Client.CanSendData == false)
                {
                    return Control;
                }
                lock (this)
                {
                    if (Control.IsFake())
                    {
                        var realControl = GetOnlineControlOrNull();
                        if (realControl != null && realControl.IsFake() == false)
                        {
                            Control = realControl;
                        }
                    }
                    return Control;
                }
            }
            catch (Exception exception)
            {
                var log = (Control.Client as Client).InternalLog;
                log.Error("Ошибка получения Control в ComponentControlWrapper", exception);
                return Control;
            }
        }

        public TControl GetInternalControl()
        {
            return Control;
        }
    }
}
