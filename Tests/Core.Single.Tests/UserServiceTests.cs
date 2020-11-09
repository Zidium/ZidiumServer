using System.Linq;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class UserServiceTests : BaseTest
    {
        [Fact]
        public void DeleteLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser(account.Id);
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                foreach (var existingUser in context.Users.ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        context.Users.Remove(existingUser);
                }
            }

            // Создадим дополнительного пользователя (не админа)

            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Проверим, что в аккаунте 1 админ
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Equal(1, adminUsers.Length);
            }

            // Проверим, что нельзя удалить единственного админа
            var storage = TestHelper.GetStorage(account.Id);
            var service = new UserService(storage);
            Assert.Throws<CantDeleteLastAdminException>(() =>
            {
                service.DeleteUser(admin.Id, account.Id);
            });

            // Проверим, что админ остался
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var existingUser = context.Users.FirstOrDefault(t => t.Id == admin.Id);
                Assert.NotNull(existingUser);
            }
        }

        [Fact]
        public void RemoveAdminRoleFromLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser(account.Id);
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                foreach (var existingUser in context.Users.ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        context.Users.Remove(existingUser);
                }
            }

            // Проверим, что в аккаунте 1 админ
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Equal(1, adminUsers.Length);
            }

            // Проверим, что нельзя удалить права админа у единственного админа
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var storage = TestHelper.GetStorage(account.Id);
                var service = new UserService(storage);
                Assert.Throws<CantRemoveAdminRoleFromLastAdmin>(() =>
                {
                    var existingAdmin = context.Users.Find(admin.Id);
                    service.RemoveUserRole(existingAdmin.Id, existingAdmin.Roles.First(t => t.RoleId == SystemRole.AccountAdministrators.Id).Id, account.Id);
                });
            }

            // Проверим, что пользователь с правами администратора остался
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var adminUsers = context.Users.Where(t => t.Roles.Any(x => x.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
                Assert.Equal(1, adminUsers.Length);
            }
        }

    }
}
