using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events.ApplicationErrors
{

    public class CreateTests : BaseTest
    {
        /// <summary>
        /// Проверим, что ошибка без поля "стек" успешно сохранится
        /// </summary>
        [Fact]
        public void NullStackTest()
        {
            var component = TestHelper.GetTestAccount().CreateRandomComponentControl();
            var response = component.CreateApplicationError("type", "message");
            Assert.Null(response.Stack);
            Assert.NotNull(response.TypeCode);
        }

        [Fact]
        public void CreateFromExceptionTest()
        {
            try
            {
                throw new Exception("Test exception");
            }
            catch (Exception e)
            {
                var account = TestHelper.GetTestAccount();
                var component = account.CreateRandomComponentControl();

                // comment + exception
                var error = component.CreateApplicationError("моя любимая ошибка", e);

                Assert.Equal(
                    "моя любимая ошибка: Exception в CreateTests.CreateFromExceptionTest() (hash 82171)",
                    error.TypeSystemName);

                Assert.Equal("Test exception", error.Message);

                Assert.NotNull(error.Stack);
                Assert.Equal("82171", error.TypeCode);

                // only exception
                error = component.CreateApplicationError(e);

                Assert.Equal(
                    "Exception в CreateTests.CreateFromExceptionTest() (hash 82171)",
                    error.TypeSystemName);

                Assert.Equal("Test exception", error.Message);

                Assert.NotNull(error.Stack);
                Assert.Equal("82171", error.TypeCode);
            }
        }

        [Fact]
        public void CreateFromExceptionWithInnerTest()
        {
            try
            {
                try
                {
                    throw new Exception("Inner exception");
                }
                catch (Exception innerException)
                {
                    throw new Exception("Test exception", innerException);
                }
            }
            catch (Exception e)
            {
                var account = TestHelper.GetTestAccount();
                var component = account.CreateRandomComponentControl();

                // comment + exception
                var error = component.CreateApplicationError("моя любимая ошибка", e);

                Assert.Equal(
                    "моя любимая ошибка: Exception в CreateTests.CreateFromExceptionWithInnerTest() (hash 84275)",
                    error.TypeSystemName);

                Assert.Equal("Test exception --> Inner exception", error.Message);

                Assert.NotNull(error.Stack);
                Assert.Equal("84275", error.TypeCode);

                // only exception
                error = component.CreateApplicationError(e);

                Assert.Equal(
                    "Exception в CreateTests.CreateFromExceptionWithInnerTest() (hash 84275)",
                    error.TypeSystemName);

                Assert.Equal("Test exception --> Inner exception", error.Message);

                Assert.NotNull(error.Stack);
                Assert.Equal("84275", error.TypeCode);
            }
        }

        [Fact]
        public void ApplicationErrorCodeTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var result = component.CreateApplicationError("TestType." + Guid.NewGuid());
            Assert.NotNull(result.TypeCode);
        }
    }
}
