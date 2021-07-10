using System;

namespace Zidium.UserAccount.Helpers
{
    public static class MyJsonHelper
    {
        public static object GetErrorResponse(Exception exception)
        {
            return new
            {
                success = false,
                error = exception.Message
            };
        }

        public static object GetSuccessResponse(object data = null)
        {
            return new
            {
                success = true,
                data = data
            };
        }
    }
}