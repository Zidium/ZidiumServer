using System.Linq;
using Xunit;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void DeleteLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser(account.Id);
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                foreach (var existingUser in userRepository.QueryAll().ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        userRepository.Remove(existingUser);
                }
            }

            // Создадим дополнительного пользователя (не админа)

            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Проверим, что в аккаунте 1 админ
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                var adminUsers = userRepository.QueryAll().Where(t => t.Roles.Any(x => x.RoleId == RoleId.AccountAdministrators)).ToArray();
                Assert.Equal(1, adminUsers.Length);
                admin = adminUsers[0];
            }

            // Проверим, что нельзя удалить единственного админа
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.Throws<CantDeleteLastAdminException>(() =>
                {
                    service.DeleteUser(service.GetById(account.Id, admin.Id), account.Id);
                });
                contexts.SaveChanges();
            }

            // Проверим, что админ остался
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                var existingUser = userRepository.QueryAll().FirstOrDefault(t => t.Id == admin.Id);
                Assert.NotNull(existingUser);
            }
        }

        [Fact]
        public void RemoveAdminRoleFromLastAdminTest()
        {
            // Удалим всех пользователей, кроме админа
            var account = TestHelper.GetTestAccount();
            var admin = TestHelper.GetAccountAdminUser(account.Id);
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                foreach (var existingUser in userRepository.QueryAll().ToArray())
                {
                    if (existingUser.Id != admin.Id)
                        userRepository.Remove(existingUser);
                }
            }

            // Проверим, что в аккаунте 1 админ
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                var adminUsers = userRepository.QueryAll().Where(t => t.Roles.Any(x => x.RoleId == RoleId.AccountAdministrators)).ToArray();
                Assert.Equal(1, adminUsers.Length);
                admin = adminUsers[0];
            }

            // Проверим, что нельзя удалить права админа у единственного админа
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.Throws<CantRemoveAdminRoleFromLastAdmin>(() =>
                {
                    var existingAdmin = service.GetById(account.Id, admin.Id);
                    service.RemoveUserRole(existingAdmin, existingAdmin.Roles.First(t => t.RoleId == RoleId.AccountAdministrators), account.Id);
                });
                contexts.SaveChanges();
            }

            // Проверим, что пользователь с правами администратора остался
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                var adminUsers = userRepository.QueryAll().Where(t => t.Roles.Any(x => x.RoleId == RoleId.AccountAdministrators)).ToArray();
                Assert.Equal(1, adminUsers.Length);
            }
        }

    }
}
