using System;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Common;

namespace Zidium.UserAccount
{
    public static class ExceptionHelper
    {
        public static void HandleException(Exception exception, ILogger logger)
        {
            if (exception == null)
                return;

            if (exception is UserFriendlyException)
                return;

            Tools.HandleOutOfMemoryException(exception);
            logger.LogError(exception, exception.Message);

            exception.HelpLink = new ExceptionRender().GetExceptionTypeCode(exception);
        }
    }
}
