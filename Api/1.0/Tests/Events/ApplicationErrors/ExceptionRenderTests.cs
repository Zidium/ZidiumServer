using System;
using System.Data.SqlClient;
using Xunit;
using Zidium.Api;

namespace ApiTests_1._0.Events.ApplicationErrors
{
    public class ExceptionRenderTests
    {
        [Fact]
        public void SqlTimeoutExceptionMessageTest()
        {
            using (var connection = new SqlConnection("Data Source=127.0.0.254;Connection Timeout=1"))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    var component = new FakeComponentControl("me");
                    var exceptionRender = new ExceptionRender();
                    var errorData = exceptionRender.GetApplicationErrorData(component, exception);
                    Assert.NotNull(errorData.Message);
                }
            }
        }
    }
}
