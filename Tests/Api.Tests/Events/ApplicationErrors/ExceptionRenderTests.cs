using Microsoft.Data.SqlClient;
using Xunit;

namespace Zidium.Api.Tests.Events.ApplicationErrors
{
    public class ExceptionRenderTests : BaseTest
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
