using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Zidium.Api
{
    public class ExceptionRender : IExceptionRender
    {
        public virtual string GetExceptionTypeJoinKey(Exception exception)
        {
            var result = "";
            while (exception != null && exception.StackTrace != null)
            {
                var stackTrace = new StackTrace(exception, false);
                var frames = stackTrace.GetFrames();
                var stack = exception.GetType().FullName + Environment.NewLine;
                if (frames != null)
                {
                    foreach (var frame in frames)
                    {
                        var frameInfo = GetFrameTextInfo(frame, true, false, true);
                        stack += " " + frameInfo + Environment.NewLine;
                    }
                }
                result += stack;
                exception = exception.InnerException;
            }
            return result.Trim();
        }

        /// <summary>
        /// Если у события есть сложенные события, то выполняется поиск самого информативного сообщения.
        ///     Например в исключении ниже вернется самое полезное сообщение о дедлоке
        /// 	System.Reflection.TargetInvocationException: Адресат вызова создал исключение. 
        /// ---> System.Data.Entity.Infrastructure.DbUpdateException: An error occurred while updating the entries. See the inner exception for details. 
        /// ---> System.Data.Entity.Core.UpdateException: An error occurred while updating the entries. See the inner exception for details. 
        /// ---> System.Data.SqlClient.SqlException: Transaction (Process ID 55) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual string GetBestInnerMessage(Exception exception)
        {
            Exception lastUselessException = null;
            var current = exception;
            while (current != null)
            {
                var currentFullType = current.GetType().FullName;
                var isUseless = _uselessTypes.Contains(currentFullType);
                if (isUseless)
                {
                    lastUselessException = current;
                }
                current = current.InnerException;
            }

            // если все сообщения "важные", то вернем все
            if (lastUselessException == null)
            {
                return GetAllMessages(exception);
            }

            // если последнее вложенное сообщение является "бесполезным", то вернем его
            if (lastUselessException.InnerException == null)
            {
                return lastUselessException.Message;
            }

            // Вернем все вложенные сообщения у последнего "бесполезного"
            return GetAllMessages(lastUselessException.InnerException);
        }

        public virtual string GetMessage(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(exception.Message))
            {
                return null;
            }
            var applicationErrorException = exception as ApplicationErrorException;
            if (applicationErrorException != null)
            {
                if (applicationErrorException.Type == applicationErrorException.Message)
                {
                    return null;
                }
            }
            if (exception.InnerException == null)
            {
                return exception.Message;
            }
            return GetBestInnerMessage(exception);
        }

        protected virtual string GetFrameTextInfo(StackFrame stackFrame, bool useFullNames, bool includeFileInfo, bool withParams)
        {
            var method = stackFrame.GetMethod();
            string fileName = null;
            if (includeFileInfo)
            {
                fileName = stackFrame.GetFileName();
                if (string.IsNullOrEmpty(fileName) == false)
                {
                    fileName = Path.GetFileName(fileName);
                }
                else
                {
                    fileName = null;
                }
            }

            var parameters = "";

            if (withParams)
            {
                var methodParameters = method.GetParameters();
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    var methodParameter = methodParameters[i];
                    if (i < methodParameters.Length - 1)
                    {
                        parameters += methodParameter.ParameterType.Name + ", ";
                    }
                    else
                    {
                        parameters += methodParameter.ParameterType.Name;
                    }
                }
            }

            var reflectedType = "UnknownType";
            if (method.ReflectedType != null)
            {
                if (useFullNames)
                {
                    reflectedType = method.ReflectedType.FullName;
                }
                else
                {
                    reflectedType = method.ReflectedType.Name;
                }
            }

            if (fileName == null)
            {
                var result = string.Format("{0}.{1}", reflectedType, method.Name);
                if (withParams)
                    result += "(" + parameters + ")";
                return result;
            }

            {
                var result = string.Format("{0}.{1}", reflectedType, method.Name);
                if (withParams)
                    result += "(" + parameters + ")";
                result += string.Format(" в {0} строка {1}", fileName, stackFrame.GetFileLineNumber());
                return result;
            }
        }

        protected virtual string GetExceptionMethodInfo(Exception exception, bool withParams)
        {
            try
            {
                if (exception == null)
                {
                    return null;
                }
                if (exception.StackTrace == null)
                {
                    return null;
                }
                var stackTrace = new StackTrace(exception, true);
                var frames = stackTrace.GetFrames();
                if (frames == null || frames.Length == 0)
                {
                    return null;
                }
                var frame = frames[0];
                const bool useFullNames = false;
                const bool includeFileInfo = false; //todo чтобы программа с PDB файлами и без генерировала одинаковые типы ошибок
                return GetFrameTextInfo(frame, useFullNames, includeFileInfo, withParams);
            }
            catch (Exception)
            {
                return "...GetStackShortInfoError";
            }
        }

        public virtual string GetExceptionTypeCode(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }
            var joinKey = GetExceptionTypeJoinKey(exception);
            var hash = HashHelper.GetInt32Dig5(joinKey);
            return hash;
        }

        protected virtual string GetTypeSystemName(string errorName, Exception exception)
        {
            string typeSystemName = null;

            // если исключения нет
            if (exception == null)
            {
                typeSystemName = errorName;
                if (typeSystemName == null)
                {
                    typeSystemName = "Неизвестный тип ошибки";
                }
                return typeSystemName;
            }

            // если исключение есть
            string type = null;
            var applicationErrorException = exception as ApplicationErrorException;
            if (applicationErrorException != null && string.IsNullOrEmpty(applicationErrorException.Type) == false)
            {
                type = applicationErrorException.Type;
            }
            else
            {
                type = exception.GetType().Name;
            }
            var methodInfo = GetExceptionMethodInfo(exception, true);

            // если стека нет
            if (string.IsNullOrEmpty(methodInfo))
            {
                typeSystemName = type;
            }
            else
            {
                // если стек есть
                var hash = GetExceptionTypeCode(exception);
                typeSystemName = type + " в " + methodInfo + " (hash " + hash + ")";
            }

            if (string.IsNullOrEmpty(errorName) == false)
            {
                typeSystemName = errorName + ": " + typeSystemName;
            }
            return typeSystemName;
        }

        protected virtual string GetTypeDisplayName(string errorName, Exception exception)
        {
            string typeDisplayName = null;

            // если исключения нет
            if (exception == null)
            {
                typeDisplayName = errorName;
                if (typeDisplayName == null)
                {
                    typeDisplayName = "Неизвестный тип ошибки";
                }
                return typeDisplayName;
            }

            // если исключение есть
            string type = null;
            var applicationErrorException = exception as ApplicationErrorException;
            if (applicationErrorException != null && string.IsNullOrEmpty(applicationErrorException.Type) == false)
            {
                type = applicationErrorException.Type;
            }
            else
            {
                type = exception.GetType().Name;
            }
            var methodInfo = GetExceptionMethodInfo(exception, false);

            // если стека нет
            if (string.IsNullOrEmpty(methodInfo))
            {
                typeDisplayName = type;
            }
            else
            {
                // если стек есть
                typeDisplayName = type + " в " + methodInfo;
            }

            if (string.IsNullOrEmpty(errorName) == false)
            {
                typeDisplayName = errorName + ": " + typeDisplayName;
            }
            return typeDisplayName;
        }

        public ApplicationErrorData GetApplicationErrorData(IComponentControl componentControl, Exception exception)
        {
            return GetApplicationErrorData(componentControl, exception, null);
        }

        protected virtual string GetAllMessages(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            string result = null;
            while (exception != null)
            {
                var message = exception.Message;
                if (string.IsNullOrEmpty(message))
                {
                    message = exception.GetType().Name;
                }
                if (result == null)
                {
                    result = message;
                }
                else
                {
                    result += " --> " + message;
                }
                exception = exception.InnerException;
            }
            return result;
        }

        public ApplicationErrorData GetApplicationErrorData(IComponentControl componentControl, Exception exception, string errorName)
        {
            var typeSystemName = GetTypeSystemName(errorName, exception);
            var typeDisplayName = GetTypeDisplayName(errorName, exception);

            var errorData = new ApplicationErrorData(componentControl, typeSystemName)
            {
                TypeDisplayName = typeDisplayName
            };

            if (exception != null)
            {
                errorData.Message = GetMessage(exception);
                errorData.Stack = GetFullStackTrace(exception);
                errorData.TypeCode = GetExceptionTypeCode(exception);

                var allMessages = GetAllMessages(exception);
                var ignoreFullMessage = allMessages == null || exception.InnerException == null || allMessages == errorData.Message;
                if (!ignoreFullMessage)
                {
                    errorData.SetProperty(ExtentionPropertyName.AllMessages, allMessages);
                }
                var internalAppException = exception as ApplicationErrorException;
                if (internalAppException != null)
                {
                    errorData.Properties.CopyFrom(internalAppException.Properties);
                }

                foreach (var dataKey in exception.Data.Keys)
                {
                    var key = dataKey.ToString();
                    var value = exception.Data[dataKey];
                    if (errorData.Properties.HasKey(key))
                    {
                        errorData.Properties[key].Value = value;
                    }
                    else
                    {
                        errorData.Properties.Set(key, value);
                    }
                }
            }
            return errorData;
        }

        public ApplicationErrorData CreateEventFromLog(IComponentControl componentControl, LogLevel level, Exception exception, string message, IDictionary<string, object> properties)
        {
            EventImportance importance;

            if (level == LogLevel.Fatal)
                importance = EventImportance.Alarm;
            else if (level == LogLevel.Warning || level == LogLevel.Error)
                importance = EventImportance.Warning;
            else if (level == LogLevel.Info)
                importance = EventImportance.Success;
            else
                importance = EventImportance.Unknown;

            ApplicationErrorData data;

            if (exception != null)
            {
                data = componentControl.CreateApplicationError(exception);
                if (data.Message != message)
                    data.Message = message + " : " + data.Message;
            }
            else
            {
                data = componentControl.CreateApplicationError("CustomError", message);
                data.SetProperty(ExtentionPropertyName.Stack, Environment.StackTrace);
            }

            data.SetImportance(importance).SetProperty("FromLog", true);

            var propertiesCollection = new ExtentionPropertyCollection(properties);
            foreach (var property in propertiesCollection)
                data.Properties.Add(property);

            return data;
        }

        public string GetFullStackTrace(Exception exception)
        {
            var result = new StringBuilder();

            while (exception != null)
            {
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    result.AppendLine(exception.GetType().FullName);
                    result.AppendLine(exception.StackTrace);
                    result.AppendLine();
                }

                exception = exception.InnerException;
            }

            return result.ToString();
        }

        /// <summary>
        /// Cписок типов исключений, которые содержат самые интересные сообщения во вложенных в них исключениях
        /// </summary>
        private static List<string> _uselessTypes = new List<string>()
        {
            "System.Reflection.TargetInvocationException",
            "System.Data.Entity.Infrastructure.DbUpdateException",
            "System.Data.SqlClient.SqlException"
        };

        protected static void AddUselessType(string typeFullName)
        {
            if (_uselessTypes.Contains(typeFullName) == false)
            {
                _uselessTypes.Add(typeFullName);
            }
        }

        protected static void RemoveUselessType(string typeFullName)
        {
            _uselessTypes.Remove(typeFullName);
        }
    }
}
