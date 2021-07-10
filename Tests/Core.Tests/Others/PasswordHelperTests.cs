using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.Others
{
    public class PasswordHelperTests : BaseTest
    {
        [Fact]
        public void GetRandomPasswordTest()
        {
            int length = 1000;
            string password = PasswordHelper.GetRandomPassword(length);
            Assert.Equal(length, password.Length);
            for (int i = 0; i < length; i++)
            {
                string charObj = password[i].ToString();
                Assert.False(string.IsNullOrWhiteSpace(charObj));
            }
        }

        [Fact]
        public void GetPasswordHashStringTest()
        {
            // данный юнит-тест нужен чтобы зафиксировать алгорит получения хэша
            var result = PasswordHelper.VerifyHashedPassword("AMTyjwjm0jS1kuREtenHVXMy0Sb7aFLRmDHaJS/PNPOYhqtMouVon7ioDtXSjOTaVQ==", "Тестовый пароль 123");
            Assert.True(result);
        }

    }
}
