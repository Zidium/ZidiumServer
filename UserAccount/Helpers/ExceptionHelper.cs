using System;
using NLog;
using Zidium.Api;
using Zidium.Common;

namespace Zidium.UserAccount
{
    public static class ExceptionHelper
    {
        public static void HandleException(Exception exception)
        {
            if (exception == null)
                return;

            if (exception is UserFriendlyException)
                return;

            Tools.HandleOutOfMemoryException(exception);
            LogManager.GetCurrentClassLogger().Error(exception);

            exception.HelpLink = new ExceptionRender().GetExceptionTypeCode(exception);
        }
    }
}
