﻿using System;
using System.IO;
using Zidium.Api;
using Xunit;

namespace ApiTests_1._0.Events.ApplicationErrors
{
    public class SystemNameTests : BaseTest
    {
        private void X1(string text, Exception x, FileInfo fileInfo)
        {
            if (text == "1") // переполнение стека
            {
                X1(text, x, fileInfo);
                return;
            }
            if (text == "2")
            {
                X2(text);
                return;
            }
            if (text == "3")
            {
                X3(text);
                return;
            }
            throw new ArgumentException("неизвестное значение параметра text");
        }

        private void X2(string text)
        {
            X3(text);
        }

        private void X3(string text)
        {
            throw new ArgumentNullException("text");
        }

        private void M3()
        {
            throw new Exception("M3 exception message");
        }

        private void M2()
        {
            try
            {
                M3();
            }
            catch (Exception exception)
            {
                throw new Exception("M2 exception message", exception);
            }
        }

        private void M1(string text, Exception x, FileInfo fileInfo)
        {
            try
            {
                M2();
            }
            catch (Exception exception)
            {
                throw new Exception("M1 exception message", exception);
            }
        }

        [Fact]
        public void MTest()
        {
            var component = new FakeComponentControl("me");
            var exceptionRender = new ExceptionRender();
            try
            {
                M1(null, null, null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception, "My test error");
                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M1(String, Exception, FileInfo)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.MTest()
System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M2()
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M1(String, Exception, FileInfo)
System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M3()
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M2()";

                Assert.Equal(joinKey1, joinKey2);

                var hash1 = HashHelper.GetInt32Dig5(joinKey2);

                var hash2 = exceptionRender.GetExceptionTypeCode(exception);
                Assert.Equal(hash1, hash2);
                Assert.Equal(hash1, error.TypeCode);

                Assert.True(error.TypeSystemName.Contains(hash1));

                Assert.Equal(
                   "My test error: Exception в SystemNameTests.M1(String, Exception, FileInfo) (hash 26195)",
                   error.TypeSystemName);

                Assert.Equal(
                   "My test error: Exception в SystemNameTests.M1",
                   error.TypeDisplayName);
            }
        }

        /// <summary>
        /// Тот же тест что и MTest, но без errorName
        /// var error = exceptionRender.GetApplicationErrorData(exception);
        /// </summary>
        [Fact]
        public void MTest2()
        {
            var component = new FakeComponentControl("me");
            var exceptionRender = new ExceptionRender();
            try
            {
                M1(null, null, null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception);
                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M1(String, Exception, FileInfo)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.MTest2()
System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M2()
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M1(String, Exception, FileInfo)
System.Exception
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M3()
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.M2()";

                Assert.Equal(joinKey1, joinKey2);

                var hash1 = HashHelper.GetInt32Dig5(joinKey2);

                var hash2 = exceptionRender.GetExceptionTypeCode(exception);
                Assert.Equal(hash1, hash2);

                Assert.True(error.TypeSystemName.Contains(hash1));

                Assert.Equal(
                   "Exception в SystemNameTests.M1(String, Exception, FileInfo) (hash 24824)",
                   error.TypeSystemName
                   );

                Assert.Equal(
                   "Exception в SystemNameTests.M1",
                   error.TypeDisplayName
                   );
            }
        }

        [Fact]
        public void XTest()
        {
            var exceptionRender = new ExceptionRender();
            var component = new FakeComponentControl("me");

            // исключение в X1
            try
            {
                X1(null, null, null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception, "My test error");
                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.ArgumentException
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X1(String, Exception, FileInfo)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.XTest()";

                Assert.Equal(joinKey1, joinKey2);

                var hash1 = HashHelper.GetInt32Dig5(joinKey2);

                var hash2 = exceptionRender.GetExceptionTypeCode(exception);
                Assert.Equal(hash1, hash2);

                Assert.True(error.TypeSystemName.Contains(hash1));

                Assert.Equal(
                   "My test error: ArgumentException в SystemNameTests.X1(String, Exception, FileInfo) (hash 83220)",
                   error.TypeSystemName
                   );

                Assert.Equal(
                   "My test error: ArgumentException в SystemNameTests.X1",
                   error.TypeDisplayName
                   );
            }

            // поймать StackOverflowException нельзя... пипец какой то

            // вызов x1=>x2=>x3 (последний кидает исключение)
            try
            {
                X1("2", null, null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception, "My test error");

                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.ArgumentNullException
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X3(String)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X2(String)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X1(String, Exception, FileInfo)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.XTest()";

                Assert.Equal(joinKey1, joinKey2);

                Assert.Equal(
                   "My test error: ArgumentNullException в SystemNameTests.X3(String) (hash 14446)",
                   error.TypeSystemName
                   );

                Assert.Equal(
                   "My test error: ArgumentNullException в SystemNameTests.X3",
                   error.TypeDisplayName
                   );
            }

            // вызов x1=>x3 (последний кидает исключение)
            try
            {
                X1("3", null, null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception, "My test error");

                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.ArgumentNullException
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X3(String)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.X1(String, Exception, FileInfo)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.XTest()";

                Assert.Equal(joinKey1, joinKey2);

                Assert.Equal(
                   "My test error: ArgumentNullException в SystemNameTests.X3(String) (hash 02471)",
                   error.TypeSystemName
                   );

                Assert.Equal(
                   "My test error: ArgumentNullException в SystemNameTests.X3",
                   error.TypeDisplayName
                   );
            }
        }

        [Fact]
        public void FileLineTest()
        {
            var exceptionRender = new ExceptionRender();
            var component = new FakeComponentControl("me");

            // исключение в X1
            try
            {
                TypeSystemNameHelper.Throw("asas", null);
            }
            catch (Exception exception)
            {
                var error = exceptionRender.GetApplicationErrorData(component, exception, "My test error");
                string joinKey1 = exceptionRender.GetExceptionTypeJoinKey(exception);

                string joinKey2 =
@"System.ArgumentNullException
 ApiTests_1._0.Events.ApplicationErrors.TypeSystemNameHelper.Throw(String, String)
 ApiTests_1._0.Events.ApplicationErrors.SystemNameTests.FileLineTest()";

                Assert.Equal(joinKey2, joinKey1);

                var hash1 = HashHelper.GetInt32Dig5(joinKey2);

                var hash2 = exceptionRender.GetExceptionTypeCode(exception);
                Assert.Equal(hash1, hash2);

                Assert.True(error.TypeSystemName.Contains(hash1));

                Assert.Equal(
                   "My test error: ArgumentNullException в TypeSystemNameHelper.Throw(String, String) (hash 79602)",
                   error.TypeSystemName
                   );

                Assert.Equal(
                   "My test error: ArgumentNullException в TypeSystemNameHelper.Throw",
                   error.TypeDisplayName
                   );
            }
        }
    }
}
