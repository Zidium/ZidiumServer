using Zidium.Core.Api;

namespace Zidium.Agent
{
    public static class LogicSettingsCache
    {
        public static GetLogicSettingsResponseData LogicSettings
        {
            get
            {
                lock (_lockObject)
                {
                    if (_logicSettings == null)
                    {
                        var dispatcherClient = DispatcherHelper.GetDispatcherClient();
                        _logicSettings = dispatcherClient.GetLogicSettings().GetDataAndCheck();
                    }

                    return _logicSettings;
                }
            }
        }

        private static GetLogicSettingsResponseData _logicSettings;

        private static readonly object _lockObject = new object();
    }
}
