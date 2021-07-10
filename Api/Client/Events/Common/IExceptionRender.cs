using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public interface IExceptionRender
    {
        ApplicationErrorData GetApplicationErrorData(
            IComponentControl componentControl,
            Exception exception);

        ApplicationErrorData GetApplicationErrorData(
            IComponentControl componentControl,
            Exception exception,
            string errorName);

        string GetExceptionTypeJoinKey(Exception exception);

        string GetExceptionTypeCode(Exception exception);

        string GetMessage(Exception exception);

        string GetFullStackTrace(Exception exception);

        ApplicationErrorData CreateEventFromLog(
            IComponentControl componentControl,
            LogLevel level,
            Exception exception,
            string message,
            IDictionary<string, object> properties);
    }
}
